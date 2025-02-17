using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;
using Serilog;
using TonedChat.Web.Models;
using TonedChat.Web.Utils;

namespace TonedChat.Web.Services;

public class ChatService
{

    private readonly ConcurrentDictionary<string, WebSocket> _clients;

    private readonly Channel<ChatMessage> _messageChannel;

    private IServiceProvider _serviceProvider;

    public ChatService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        
        _clients = new ConcurrentDictionary<string, WebSocket>();
        _messageChannel = Channel.CreateUnbounded<ChatMessage>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false,
        });
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

    private async Task ProcessMessage(byte[] messageBytes)
    {
        var messageString = Encoding.UTF8.GetString(messageBytes);
        Log.Information("Received the following message from client: " + messageString);

        var message = TautSerializer.Deserialize<ChatMessage>(messageString);
        if (message == null)
        {
            throw new Exception("Unable to process message, incorrect format");
        }
        
        // TODO: Perform some validation of this message

        // Set the ID of our message
        message.Id = Guid.NewGuid();
        
        await _messageChannel.Writer.WriteAsync(message, CancellationToken.None);
    }

    private Func<Task> CreateClientReadThread(string connectionId, WebSocket ws)
    {
        var chatService = this;
        return async () =>
        {
            var buffer = new byte[1024 * 4];

            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
                
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    Log.Warning("Received a binary message ,which we do not support, closing WS");
                    await ws.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot accept binary",
                        CancellationToken.None);
                    break;
                }
                
                if (!result.EndOfMessage)
                {
                    Log.Warning("Message overflowed our buffer, closing connection");
                    await ws.CloseAsync(WebSocketCloseStatus.MessageTooBig, "Message exceeded buffer. Goodbye",
                        CancellationToken.None);
                }
 
                var stringBytes = new byte[result.Count];
                Array.Copy(buffer, 0, stringBytes, 0, result.Count);
      
                await ProcessMessage(stringBytes);
            }

            Log.Information("Connection was closed");
            chatService.RemoveClient(connectionId);
        };
    }

    public async Task SendMessagesWork(CancellationToken cancellationToken)
    {
        var reader = _messageChannel.Reader;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!await reader.WaitToReadAsync(cancellationToken))
                {
                    // if wait to ready async returns false, it means there will never be data to read from the channel
                    //  again (as it is closed) Nothing more to do here, eject
                    break;
                }


                using var scope = _serviceProvider.CreateScope();
                var chatMessageService = scope.ServiceProvider.GetRequiredService<ChatMessageService>();
                while (reader.TryRead(out var message) && !cancellationToken.IsCancellationRequested)
                {
                    // TODO: We should probably add the chat to the DB before we add to the channel?
                    var dbTask = chatMessageService.AddMessage(message);
    
                    var messageString = TautSerializer.Serialize(message);
                    var dispatchTask = DispatchMessage(Encoding.UTF8.GetBytes(messageString), cancellationToken);
                    await Task.WhenAll(dbTask, dispatchTask);
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

}