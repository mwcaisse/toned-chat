using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;
using Serilog;
using TonedChat.Web.Models.Messaging;
using TonedChat.Web.Services.Messaging.Processing;
using TonedChat.Web.Utils;

namespace TonedChat.Web.Services;

public class MessageService
{

    private readonly ConcurrentDictionary<string, WebSocket> _clients;

    private readonly IServiceProvider _serviceProvider;

    private readonly CancellationTokenSource _applicationStopTokenSource;

    private readonly MessageQueue _messageQueue;

    public MessageService(IServiceProvider serviceProvider, MessageQueue messageQueue)
    {
        _serviceProvider = serviceProvider;
        _messageQueue = messageQueue;

        _clients = new ConcurrentDictionary<string, WebSocket>();
        _applicationStopTokenSource = new CancellationTokenSource();
    }

    public async Task<Func<Task>> AddNewClient(HttpContext context)
    {
        var ws = await context.WebSockets.AcceptWebSocketAsync();
        var connectionId = context.Connection.Id;

        _clients[connectionId] = ws;

        return CreateClientReadThread(connectionId, ws);
    }

    private void RemoveClient(string connectionId)
    {
        _clients.TryRemove(connectionId, out _);
    }

    private IMessageProcessor? GetMessageProcessorForMessage(Message message, IServiceProvider serviceProvider)
    {
        return serviceProvider.GetKeyedService<IMessageProcessor>(message.Type);
    }
    
    private async Task ProcessMessage(byte[] messageBytes, CancellationToken cancellationToken)
    {
        var messageString = Encoding.UTF8.GetString(messageBytes);
        Log.Information("Received the following message from client: " + messageString);

        var message = TautSerializer.Deserialize<Message>(messageString);
        if (message == null)
        {
            throw new Exception("Unable to process message, incorrect format");
        }

        using var scope = _serviceProvider.CreateScope();
        var messageProcessor = GetMessageProcessorForMessage(message, scope.ServiceProvider);
        if (messageProcessor == null)
        {
            Log.Warning("Could not find a message processor for message type {messageType}", message.Type);
            return;
        }

        await messageProcessor.ProcessMessage(message, cancellationToken);
    }

    private Func<Task> CreateClientReadThread(string connectionId, WebSocket ws)
    {
        var chatService = this;
        var cancellationToken = _applicationStopTokenSource.Token;
        return async () =>
        {
            try
            {
                var buffer = new byte[1024 * 4];

                while (ws.State == WebSocketState.Open)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken);
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        Log.Warning("Received a binary message ,which we do not support, closing WS");
                        await ws.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary",
                            cancellationToken);
                        break;
                    }

                    if (!result.EndOfMessage)
                    {
                        Log.Warning("Message overflowed our buffer, closing connection");
                        await ws.CloseAsync(WebSocketCloseStatus.MessageTooBig, "Message exceeded buffer. Goodbye",
                            cancellationToken);
                    }

                    var stringBytes = new byte[result.Count];
                    Array.Copy(buffer, 0, stringBytes, 0, result.Count);

                    await ProcessMessage(stringBytes, cancellationToken);
                }
            }
            catch (OperationCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Log.Information("Closing connection with client {connectionId} as application is stopping", connectionId);
                }
                else
                {
                    Log.Error(ex, "Other cancellation occured for connection ({connectionId})", connectionId);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured while listening to connection ({connectionId}) for client", connectionId);
            }
            finally
            {
                Log.Information("Connection was closed");
                chatService.RemoveClient(connectionId);
            }
        };
    }

    public async Task SendMessagesWork()
    {
        var cancellationToken = _applicationStopTokenSource.Token;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!await _messageQueue.WaitForMessage(cancellationToken))
                {
                    // if wait to ready async returns false, it means there will never be data to read from the channel
                    //  again (as it is closed) Nothing more to do here, eject
                    break;
                }

                using var scope = _serviceProvider.CreateScope();

                while (_messageQueue.ReadMessage(out var message) && !cancellationToken.IsCancellationRequested)
                {
                    var messageString = TautSerializer.Serialize(message);
                    await DispatchMessage(Encoding.UTF8.GetBytes(messageString), cancellationToken);
                }
            }
            catch (OperationCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Log.Information("Stopping message dispatch thread as the application is stopping.");
                }
                else
                {
                    Log.Error(ex, "Stopping message dispatch thread from an unexpected cancellation");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed when processing dispatched messages.");
            }
        }
    }

    private async Task DispatchMessage(byte[] message, CancellationToken cancellationToken)
    {
        foreach (var ws in _clients.Values)
        {
            await ws.SendAsync(message, WebSocketMessageType.Text, true, cancellationToken);
        }
    }

    public void Stop()
    {
        _applicationStopTokenSource.Cancel();
    }
}