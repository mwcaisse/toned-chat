using TonedChat.Web.Models.Messaging;

namespace TonedChat.Web.Services.Messaging.Processing;

public interface IMessageProcessor
{
    Task ProcessMessage(Message message, CancellationToken cancellationToken = default);
}