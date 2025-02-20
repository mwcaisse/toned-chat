import { ChatService } from "@app/services/ChatService";
import {createContext} from "react";


export type ChatContextType = {
    chatService: ChatService ;
}

export const ChatContext = createContext({} as ChatContextType);
