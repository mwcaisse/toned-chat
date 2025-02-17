import {
    Box,
    TextInput,
    Text,
    ScrollArea,
    Paper,
    Button,
    Group,
    Flex,
    Affix,
    Transition, HoverCard
} from "@mantine/core";
import {IconX} from '@tabler/icons-react';
import {useEffect, useRef, useState} from "react";
import StringUtils from "@app/utils/StringUtils.ts";
import {ChatService, ListenerDelegate} from "@app/services/ChatService.ts";
import {KeyboardEvent} from "react";
import {Message} from "@app/models/Chat.ts";
import {DateTime} from "luxon";
import {notifications} from "@mantine/notifications";

const chatService = new ChatService();

function Chat() {

    const [name, setName] = useState("Mitchell");
    const [currentMessage, setCurrentMessage] = useState<string>("");
    const [messages, setMessages] = useState<Message[]>([]);
    const scrollViewport = useRef<HTMLDivElement>(null);
    const [scrolledToBottom, setScrolledToBottom] = useState<boolean>(true);

    const send = () => {
        try {
            chatService.send(name, currentMessage);
            setCurrentMessage("");
        }
        catch (error: any) {
            console.error("Could not send message", error);
            notifications.show({
                position: "top-right",
                autoClose: 3000,
                color: "red",
                icon: <IconX />,
                title: "Could not send message",
                message: "Failed to send message: " + error.message,
            })
        }
    };

    const formatMessageDate = (d: string): string => {
        const dt = DateTime.fromISO(d);

        const dtLocal = dt.toLocal().startOf("day");
        const now = DateTime.now().startOf("day");
        const dayDiff = now.diff(dtLocal, "days").days;

        let dateDisplay = "";
        if (dayDiff < 1) {
            dateDisplay = "";
        }
        else if (dayDiff < 2) {
            dateDisplay = "Yesterday";
        }
        else if (dayDiff < 7) {
            dateDisplay = dtLocal.toFormat("EEEE");
        }
        else {
            dateDisplay = dt.toLocaleString(DateTime.DATE_FULL);
        }

        const timeDisplay = dt.toLocaleString(DateTime.TIME_24_WITH_SECONDS);

        if (StringUtils.isNullOrWhitespace(dateDisplay)) {
            return timeDisplay;
        }
        return `${dateDisplay}, ${timeDisplay}`;
    };

    const formatMessageDateFull = (d: string): string => {
        const dt = DateTime.fromISO(d);
        return `${dt.toLocaleString(DateTime.DATE_FULL)} at ${dt.toLocaleString(DateTime.TIME_24_WITH_SECONDS)}`;
    };

    const scrollToBottom = () => {
        if (scrollViewport.current) {
            console.dir(scrollViewport.current);
            scrollViewport.current.scrollTo({top: scrollViewport.current.scrollHeight, behavior: "instant"});
            setScrolledToBottom(true);
        }
    }

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
            const scrollTopMax = scrollViewport.current.scrollHeight - scrollViewport.current.clientHeight;
            setScrolledToBottom(scrollTopMax - y < 5);
        }
    };

    return (
        <>
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
                                <Text fw="bold">{message.userName}</Text>
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
            <Affix position={{ bottom: 80, right: 20}}>
                <Transition mounted={!scrolledToBottom} transition="slide-up">
                    {(transitionStyles) => (
                        <Button
                            onClick={() => scrollToBottom()}
                            style={transitionStyles}
                        >
                            Scroll to Bottom
                        </Button>
                    )}
                </Transition>
            </Affix>
        </>


    )
}

export default Chat;