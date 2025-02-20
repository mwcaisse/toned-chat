using TonedChat.Web.Services;

namespace TonedChat.Web.Endpoints;

public static class ChatEndpoints
{
    public static void RegisterChatEndpoints(this WebApplication app)
    {
        app.Map("/chat/ws", ChatWebSocket);
        app.MapGet("/channel/", GetChannels);
        app.Map("/channel/{channelId}/messages", GetMessagesForChannel);
    }

    static async Task<IResult> ChatWebSocket(HttpContext context, MessageService messageService, CancellationToken cancellationToken)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            return TypedResults.BadRequest();
        }

        var readTask = await messageService.AddNewClient(context);
        await readTask();

        // there will already be a result when the WS is closed, so we don't need to do anything
        return TypedResults.Empty;
    }
    
    static IResult GetChannels(ChatChannelService chatChannelService)
    {
        var allChannels = chatChannelService.GetAll();
        return Results.Ok(allChannels);
    }

    static IResult GetMessagesForChannel(Guid channelId, ChatMessageService chatMessageService)
    {
        var messagesForChannel = chatMessageService.GetAllForChannel(channelId);
        return Results.Ok(messagesForChannel);
    }
}