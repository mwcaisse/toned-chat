import { DateTime } from "luxon";
import {Channel, Message} from "@app/models/Chat.ts";
import axios from "axios";

export type ListenerDelegate = (message: Message) => void;

export class ChatService {
    ws: WebSocket;
    listeners: ListenerDelegate[];

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
            type: "SEND_CHAT_MESSAGE",
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

        if (message.type === "RECEIVE_CHAT_MESSAGE") {
            for (const listener of this.listeners) {
                listener(message.payload);
            }
        }
        else {
            console.log(`Received a message of type ${message.type} but we aren't processing it now`);
            console.dir(message)
        }
    }

    addListener(listener: ListenerDelegate): void {
        this.listeners.push(listener)
    }

    removeListener(listener: ListenerDelegate): void {
        const index = this.listeners.indexOf(listener);
        if (index > -1) {
            this.listeners.splice(index, 1)
        }
    }

    getHistoricalForChannel(channelId: string): Promise<Message[]> {
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

        // TODO: Need to add a type enum for messages
        const message = {
            id: crypto.randomUUID(),
            type: "CREATE_CHANNEL",
            payload:  {
                name
            }
        };
        this.ws.send(JSON.stringify(message));
    }
}