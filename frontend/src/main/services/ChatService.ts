

class ChatService {
    ws: WebSocket;

    constructor() {
        this.ws = new WebSocket("ws://localhost:5136/chat/ws");

        this.ws.addEventListener("message", (event) => {
            console.log("Message from server: ",  event.data);
        })
    }

    send(message: string): void {
        this.ws.send(message)
    }
}

export default ChatService;