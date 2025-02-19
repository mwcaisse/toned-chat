using Serilog;

namespace TonedChat.Web.Services;

public class MessageDispatchBackgroundService : BackgroundService
{
    private readonly MessageService _messageService;
    
    public MessageDispatchBackgroundService(MessageService messageService)
    {
        _messageService = messageService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork();
    }

    private async Task DoWork()
    {
        try
        {
            await _messageService.SendMessagesWork();
        }
        catch (Exception e)
        {
            Log.Error(e, "Caught an unhandled exception in {className}. The service is dead now.", GetType().Name);
        }
    }
}