using System.Text.Json.Serialization;

namespace TonedChat.Web.Models.Messaging;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ChannelCreatedMessage), typeDiscriminator: ChannelCreatedMessage.TYPE)]
[JsonDerivedType(typeof(CreateChannelMessage), typeDiscriminator: CreateChannelMessage.TYPE)]
[JsonDerivedType(typeof(ReceiveChatMessage), typeDiscriminator: ReceiveChatMessage.TYPE)]
[JsonDerivedType(typeof(SendChatMessage), typeDiscriminator: SendChatMessage.TYPE)]
public abstract class Message
{
    protected Message()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; init; }

    public abstract string Type { get; }

}

public abstract class MessageWithPayload<T> : Message
{
    public T Payload { get; init; }
}