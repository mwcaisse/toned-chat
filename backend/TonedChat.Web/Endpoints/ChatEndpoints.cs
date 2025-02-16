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
        app.MapGet("/chat/historical", GetHistoricalMessages);
    }

    static async Task<IResult> ChatWebSocket(HttpContext context, ChatService chatService)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            return TypedResults.BadRequest();
        }

        var readTask = await chatService.AddNewClient(context);
        await readTask();
        
        Log.Information("Connection was closed :(");

        // there will already be a result when the WS is closed, so we don't need to do anything
        return TypedResults.Empty;
    }

    static IResult GetHistoricalMessages(ChatHistoryService chatHistoryService)
    {
        var allMessages = chatHistoryService.GetHistoricalMessages();
        return Results.Ok(allMessages);
    }
}