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
        app.MapGet("/chat/", GetMessages);
    }

    static async Task<IResult> ChatWebSocket(HttpContext context, ChatService chatMessageService, CancellationToken cancellationToken)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            return TypedResults.BadRequest();
        }

        var readTask = await chatMessageService.AddNewClient(context);
        await readTask();

        // there will already be a result when the WS is closed, so we don't need to do anything
        return TypedResults.Empty;
    }

    static IResult GetMessages(ChatMessageService chatMessageService)
    {
        var allMessages = chatMessageService.GetAll();
        return Results.Ok(allMessages);
    }
}