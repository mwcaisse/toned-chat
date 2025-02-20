import { DateTime } from "luxon";
import {Channel, ChatMessage} from "@app/models/Chat.ts";
import axios from "axios";
import {Message, MessageTypes} from "@app/models/Messages.ts";

export type ListenerDelegate = (message: Message) => void;

export type MessageListener = {
    messageTypes: Set<MessageTypes>;
    onMessage: ListenerDelegate;
}

export class ChatService {
    ws: WebSocket;
    listeners: MessageListener[];

    constructor() {
        this.ws = new WebSocket("ws://localhost:5136/chat/ws");
        this.listeners = [];

        this.ws.addEventListener("message", (event) => {
            this.notify(event.data);
        })
    }

    send(channelId: string, name: string, messageText: string): void {
        if (this.ws.readyState !== WebSocket.OPEN) {
            throw new Error("No connection to the server.");
        }

        const message = {
            id: crypto.randomUUID(),
            type: MessageTypes.SendChatMessage,
            payload:  {
                channelId: channelId,
                userName: name,
                content: messageText,
                date: DateTime.utc().toJSON()
            }
        };
        this.ws.send(JSON.stringify(message));
    }

    notify(messageJson: string): void {
        const message = JSON.parse(messageJson);

        for (const listener of this.listeners) {
            if (listener.messageTypes.has(message.type)) {
                listener.onMessage(message);
            }
        }
    }

    addListener(listener: MessageListener): void {
        this.listeners.push(listener)
    }

    removeListener(listener: MessageListener): void {
        const index = this.listeners.indexOf(listener);
        if (index > -1) {
            this.listeners.splice(index, 1)
        }
    }

    getHistoricalForChannel(channelId: string): Promise<ChatMessage[]> {
        const url = `http://localhost:5136/channel/${channelId}/messages`;

        return axios.get(url).then(
            res => res.data,
            error => {
                console.error("Error when trying to get historical messages", error);
                throw error;
            }
        )
    }

    getChannels(): Promise<Channel[]> {
        const url = "http://localhost:5136/channel/";

        return axios.get(url).then(
            res => res.data,
            error => {
                console.error("Error when trying to get all channels", error);
                throw error;
            }
        )
    }

    createChannel(name: string): void {
        if (this.ws.readyState !== WebSocket.OPEN) {
            throw new Error("No connection to the server.");
        }

        const message = {
            id: crypto.randomUUID(),
            type: MessageTypes.CreateChannel,
            payload:  {
                name
            }
        };
        this.ws.send(JSON.stringify(message));
    }
}