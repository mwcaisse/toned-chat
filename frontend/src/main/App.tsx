import Chat from "@app/components/Chat.tsx";
import {AppShell, Group} from "@mantine/core";
import ChannelList from "@app/components/ChannelList.tsx";
import {useState} from "react";
import {ChatContext, ChatContextType} from "./context/ChatContext";
import {ChatService} from "@app/services/ChatService.ts";


const chatService = new ChatService();
const chatContext: ChatContextType = {
    chatService
};

function App() {

    const [activeChannelId, setActiveChannelId] = useState<string | null>(null);

    return (
        <ChatContext.Provider
            value={chatContext}
        >
            <AppShell
                header={{height: 60}}
                navbar={{
                    width: 200,
                    breakpoint: "sm"
                }}
                padding="md"
            >
                <AppShell.Header>
                    <Group h="100%" px="md">
                        <h3>Toned Chat</h3>
                    </Group>
                </AppShell.Header>
                <AppShell.Navbar>
                    <ChannelList
                        activeChannelId={activeChannelId}
                        setActiveChannelId={setActiveChannelId}
                    />
                </AppShell.Navbar>
                <AppShell.Main>
                    <Chat
                        activeChannelId={activeChannelId}
                    />
                </AppShell.Main>
            </AppShell>
        </ChatContext.Provider>

    )
}

export default App
