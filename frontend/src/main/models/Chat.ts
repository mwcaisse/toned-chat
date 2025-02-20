export type ChatMessage = {
    id: string;
    channelId: string;
    userName: string;
    content: string;
    date: string
}

export type Channel = {
    id: string;
    name: string;
}

export type TypingIndicator = {
    channelId: string;
    user: string
}