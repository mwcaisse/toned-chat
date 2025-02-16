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

    static async Task<Results<Ok, BadRequest>> ChatWebSocket(HttpContext context, ChatService chatService)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            return TypedResults.BadRequest();
        }

        var readTask = await chatService.AddNewClient(context);
        await readTask();
        
        Log.Information("Connection was closed :(");
        
        // Do we return anything here though?
        return TypedResults.Ok();
    }
}