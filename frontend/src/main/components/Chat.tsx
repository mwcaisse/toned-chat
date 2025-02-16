import {Box, TextInput, Text, ScrollArea, Paper, Button, Group, Flex, HoverCard} from "@mantine/core";
import {useEffect, useRef, useState} from "react";
import StringUtils from "@app/utils/StringUtils.ts";
import {ChatService, ListenerDelegate} from "@app/services/ChatService.ts";
import {KeyboardEvent} from "react";
import {Message} from "@app/models/Chat.ts";
import {DateTime} from "luxon";

const chatService = new ChatService();

function Chat() {

    const [name, setName] = useState("Mitchell");
    const [currentMessage, setCurrentMessage] = useState<string>("");
    const [messages, setMessages] = useState<Message[]>([]);
    const scrollViewport = useRef<HTMLDivElement>(null);
    const [scrolledToBottom, setScrolledToBottom] = useState<boolean>(true);

    const send = () => {
        chatService.send(name, currentMessage);
        setCurrentMessage("");
    };

    const formatMessageDate = (d: string): string => {
        const dt = DateTime.fromISO(d);
        return dt.toLocaleString(DateTime.TIME_24_WITH_SECONDS);
    };

    const formatMessageDateFull = (d: string): string => {
        const dt = DateTime.fromISO(d);
        return `${dt.toLocaleString(DateTime.DATE_FULL)} at ${dt.toLocaleString(DateTime.TIME_24_WITH_SECONDS)}`;
    };

    // fetch any historical messages
    useEffect(() => {
        const fetch = async () => {
            const historicalMessages = await chatService.getHistorical();
            setMessages(historicalMessages)
        };

        fetch()
            .catch(console.error);

    }, []);

    // sign up for events from the WS
    useEffect(() => {
        const listener: ListenerDelegate = (messageData) => {
            const message = JSON.parse(messageData);
            setMessages([...messages, message])
        }

        chatService.addListener(listener);
        return () => {
            chatService.removeListener(listener)
        }
    });

    // whenever messages changes, scroll to the bottom
    useEffect(() => {
        if (!scrollViewport.current) {
            return;
        }
        const current = scrollViewport.current;

        // this executes after messages has been changed, so this will never be true
        if (scrolledToBottom) {
            current.scrollTo({top: current.scrollHeight, behavior: "instant"});
        }
    }, [messages])

    const handleKeyPress = (event: KeyboardEvent<HTMLDivElement>) => {
        if (event.key === "Enter" && !StringUtils.isNullOrEmpty(currentMessage)) {
            event.preventDefault();
            event.stopPropagation();
            send();
        }
    };

    const handleScrolled = ({y} : {y : number}) => {
        if (scrollViewport.current) {
            setScrolledToBottom(scrollViewport.current.scrollTopMax - y < 5);
        }
    };

    return (
        <Flex direction="column" styles={{
            root: {
                minHeight: "calc(100vh - 100px)",
            }
        }}>
            <Box>
                <TextInput
                    label="Who are you?"
                    styles={{
                        wrapper: {
                            flexGrow: 10
                        }
                    }}
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    onKeyDown={handleKeyPress}
                />
            </Box>
            <ScrollArea
                styles={{
                    root: {
                        flexGrow: 10,
                        height: "calc(100vh - 220px)",
                    }
                }}
                viewportRef={scrollViewport}
                onScrollPositionChange={handleScrolled}
            >
                {messages.map((message) =>
                    <Paper shadow="xs" p="xs" m="xs" key={message.id}>
                        <Group>
                            <Text fw="bold">{message.name}</Text>
                            <HoverCard openDelay={500} withArrow position="top">
                                <HoverCard.Target>
                                    <Text>{formatMessageDate(message.date)}</Text>
                                </HoverCard.Target>
                                <HoverCard.Dropdown>
                                    <Text>{formatMessageDateFull(message.date)}</Text>
                                </HoverCard.Dropdown>
                            </HoverCard>

                        </Group>
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
                    onKeyDown={handleKeyPress}
                />
            </Box>
        </Flex>

    )
}

export default Chat;