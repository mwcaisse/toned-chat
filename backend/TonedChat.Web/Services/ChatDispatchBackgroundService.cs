using Serilog;

namespace TonedChat.Web.Services;

public class ChatDispatchBackgroundService : BackgroundService
{
    private readonly ChatService _chatMessageService;
    
    public ChatDispatchBackgroundService(ChatService chatMessageService)
    {
        _chatMessageService = chatMessageService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork();
    }

    private async Task DoWork()
    {
        try
        {
            await _chatMessageService.SendMessagesWork();
        }
        catch (Exception e)
        {
            Log.Error(e, "Caught an unhandled exception in {className}. The service is dead now.", GetType().Name);
        }
    }
}