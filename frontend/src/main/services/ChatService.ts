import { DateTime } from "luxon";
import {Message} from "@app/models/Chat.ts";
import axios from "axios";

export type ListenerDelegate = (e: string) => void;

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

    send(name: string, messageText: string): void {
        const message = {
            userName: name,
            content: messageText,
            date: DateTime.utc().toJSON()
        };
        this.ws.send(JSON.stringify(message));
    }

    notify(messageData: string): void {
        for (const listener of this.listeners) {
            listener(messageData);
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

    getHistorical(): Promise<Message[]> {
        const url = "http://localhost:5136/chat/";

        return axios.get(url).then(
            res => res.data,
            error => {
                console.error("Error when trying to get historical messages", error);
                throw error;
            }
        )

    }
}