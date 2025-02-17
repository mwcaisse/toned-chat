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
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        try
        {
            await _chatMessageService.SendMessagesWork(stoppingToken);
        }
        catch (Exception e)
        {
            Log.Error(e, "Caught an unhandled exception in {className}. The service is dead now.", GetType().Name);
        }
    }
}