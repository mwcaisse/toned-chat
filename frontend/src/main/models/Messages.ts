export enum MessageTypes {
    SendChatMessage = "SEND_CHAT_MESSAGE",
    ReceiveChatMessage = "RECEIVE_CHAT_MESSAGE",
    CreateChannel = "CREATE_CHANNEL",
    ChannelCreated = "CHANNEL_CREATED",
    StartedTyping = "STARTED_TYPING",
    StoppedTyping = "STOPPED_TYPING",
}

export type Message = {
    id: string;
    type: string
}

export interface MessageWithPayload<T> extends Message {
    payload: T;
}