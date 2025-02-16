import {Box, TextInput, Text, ScrollArea, Paper, Button, Group, Flex} from "@mantine/core";
import {useEffect, useState} from "react";
import StringUtils from "@app/utils/StringUtils.ts";
import {ChatService, ListenerDelegate} from "@app/services/ChatService.ts";

type Message = {
    name: string;
    content: string;
    date: string
}

const chatService = new ChatService();

function Chat() {

    const [currentMessage, setCurrentMessage] = useState<string>("");
    const [messages, setMessages] = useState<Message[]>([]);

    const send = () => {
        chatService.send("Mitchell", currentMessage);
        setCurrentMessage("");
    }

    // sign up for events from the WS
    useEffect(() => {
        const listener: ListenerDelegate = (messageData) => {
            console.log("sad");
            console.log(messageData)
            const message = JSON.parse(messageData);
            setMessages([...messages, message])
        }

        chatService.addListener(listener);
        return () => {
            chatService.removeListener(listener)
        }
    });

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
                        <Text size="xs">{message.name} on {message.date} </Text>
                        <Text size="md">{message.content}</Text>
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