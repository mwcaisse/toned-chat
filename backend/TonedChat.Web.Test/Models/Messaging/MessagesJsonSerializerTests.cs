using TonedChat.Web.Models;
using TonedChat.Web.Models.Messaging;
using TonedChat.Web.Utils;

namespace TonedChat.Web.Test.Models.Messaging;

public class MessagesJsonSerializerTests
{
    [Fact]
    public void CanDeserializeMessage()
    {
        var message = new CreateChannelMessage()
        {
            Id = Guid.NewGuid(),
            Payload = new ChatChannel()
            {
                Name = "general"
            }
        };

        var serialized = TautSerializer.Serialize(message);
        var deserialized = TautSerializer.Deserialize<Message>(serialized);

        Assert.NotNull(deserialized);
        Assert.True(deserialized is CreateChannelMessage);

        var deserializedMessage = (CreateChannelMessage)deserialized;
        Assert.Equal("general", deserializedMessage.Payload.Name);
    }
    
    [Fact]
    public void CanDeserializeSendChatMessage()
    {
        var message = new SendChatMessage()
        {
            Id = Guid.NewGuid(),
            Payload = new ChatMessage()
            {
                UserName = "mitchell",
                Content = "test"
            }
        };

        var serialized = TautSerializer.Serialize(message);
        var deserialized = TautSerializer.Deserialize<Message>(serialized);

        Assert.NotNull(deserialized);
        Assert.True(deserialized is SendChatMessage);

        var deserializedMessage = (SendChatMessage)deserialized;
        Assert.Equal("test", deserializedMessage.Payload.Content);
    }

    [Fact]
    public void CanDeserializeMessageFromStringWhenTypeIsNotFirst()
    {
        var messageString =
            @"{""id"":""e7afe58e-f1cc-410d-8a7b-5d71c3ab2c94"",""type"":""SEND_CHAT_MESSAGE"",""payload"":{""userName"":""Mitchell"",""content"":""test"",""date"":""2025-02-18T04:41:34.999Z""}}";


        var deserialized = TautSerializer.Deserialize<Message>(messageString);

        Assert.NotNull(deserialized);
        Assert.True(deserialized is SendChatMessage);

        var deserializedMessage = (SendChatMessage)deserialized;
        Assert.Equal("test", deserializedMessage.Payload.Content);
    }
}