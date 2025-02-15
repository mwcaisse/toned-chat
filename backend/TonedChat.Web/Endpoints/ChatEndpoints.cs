using System.Net.WebSockets;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TonedChat.Web.Endpoints;

public static class ChatEndpoints
{
    public static void RegisterChatEndpoints(this WebApplication app)
    {
        app.Map("/chat/ws", ChatWebSocket);
    }

    static async Task<Results<Ok, BadRequest>> ChatWebSocket(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            return TypedResults.BadRequest();
        }

        var buffer = new byte[1024 * 4];
        var ws = await context.WebSockets.AcceptWebSocketAsync();

        WebSocketReceiveResult? result = null;
        while (!ws.CloseStatus.HasValue)
        {
            result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        if (result != null)
        {
            await ws.CloseAsync(result.CloseStatus!.Value, result.CloseStatusDescription, CancellationToken.None);    
        }
        else
        {
            await ws.CloseAsync(WebSocketCloseStatus.InternalServerError, "Borked", CancellationToken.None);
        }
        
        
        // Do we return anything here though?
        return TypedResults.Ok();
    }
}