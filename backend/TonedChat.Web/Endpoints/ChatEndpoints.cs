using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;
using TonedChat.Web.Services;

namespace TonedChat.Web.Endpoints;

public static class ChatEndpoints
{
    public static void RegisterChatEndpoints(this WebApplication app)
    {
        app.Map("/chat/ws", ChatWebSocket);
    }

    static async Task<Results<Ok, BadRequest>> ChatWebSocket(HttpContext context, ChatMessageCollection messageCollection)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            return TypedResults.BadRequest();
        }

        var buffer = new byte[1024 * 4];
        var ws = await context.WebSockets.AcceptWebSocketAsync();
        
        var lastMessage = -1;
        
        // let's send them all of the messages that happened before they connected
        foreach (var message in messageCollection.GetMessageSinceLast(lastMessage))
        {
            await ws.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true,
                CancellationToken.None);
            lastMessage++;
        }
        
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
            
            //we have a text message now

            var stringBytes = new byte[result.Count];
            Array.Copy(buffer, 0, stringBytes, 0, result.Count);

            var message = System.Text.Encoding.UTF8.GetString(stringBytes);
            Log.Information("Received the following message from client: " + message);
            
            //add the message to our collection
            messageCollection.Add(message);
            
            // now we are going to echo it back to them,
            await ws.SendAsync(stringBytes, WebSocketMessageType.Text, true, CancellationToken.None);
            
        }
        
        Log.Information("Connection was closed :(");
        
        // Do we return anything here though?
        return TypedResults.Ok();
    }
}