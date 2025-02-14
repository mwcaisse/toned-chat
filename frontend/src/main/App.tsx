import Chat from "@app/components/Chat.tsx";
import {AppShell, Group} from "@mantine/core";


function App() {

    return (
        <AppShell
            header={{height: 60}}
            padding="md"
        >
            <AppShell.Header>
                <Group h="100%" px="md">
                    <h3>Toned Chat</h3>
                </Group>
            </AppShell.Header>
            <AppShell.Main>
                <Chat />
            </AppShell.Main>
        </AppShell>

    )
}

export default App
