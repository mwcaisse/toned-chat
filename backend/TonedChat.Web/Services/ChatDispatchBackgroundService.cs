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
        // TODO: Need some error hanlding here, so we don't crash the whole webserver if this breaks
        await _chatMessageService.SendMessagesWork(stoppingToken);
    }
}