import {Box, TextInput, Text, ScrollArea, Paper, Button, Group, Flex} from "@mantine/core";
import {useState} from "react";
import StringUtils from "@app/utils/StringUtils.ts";
import ChatService from "@app/services/ChatService.ts";

type Message = {
    user: string;
    message: string;
}

const chatService = new ChatService();

function Chat() {

    const [currentMessage, setCurrentMessage] = useState<string>("");

    const messages: Message[] = [
        {
            user: "Mitchell 2",
            message: "Hello!"
        },
        {
            user: "Mitchell",
            message: "World!"
        },
        {
            user: "Mitchell",
            message: "This is life!!"
        },
    ];

    const send = () => {
        chatService.send(currentMessage);
        setCurrentMessage("");
    }

    return (
        <Flex direction="column" styles={{
            root: {
                minHeight: "calc(100vh - 100px)",
            }
        }}>
            <ScrollArea styles={{
                root: {
                    flexGrow: 10
                }
            }}>
                {messages.map((message) =>
                    <Paper shadow="xs" p="xs" m="xs">
                        <Text size="xs">{message.user}</Text>
                        <Text size="md">{message.message}</Text>
                    </Paper>
                )}
            </ScrollArea>

            <Box>
                <TextInput
                    label="Chat"
                    styles={{
                        wrapper: {
                            flexGrow: 10
                        }
                    }}
                    inputContainer={(children) => (
                        <Group>
                            {children}
                            <Button
                                onClick={() => send()}
                                disabled={StringUtils.isNullOrEmpty(currentMessage)}
                            >
                                Send
                            </Button>
                        </Group>
                    )}
                    value={currentMessage}
                    onChange={(e) => setCurrentMessage(e.target.value)}
                />
            </Box>
        </Flex>

    )
}

export default Chat;