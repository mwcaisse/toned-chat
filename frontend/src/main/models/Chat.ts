export type Message = {
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