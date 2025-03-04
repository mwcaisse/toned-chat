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
import {useContext, useEffect, useRef, useState} from "react";
import StringUtils from "@app/utils/StringUtils.ts";
import {MessageListener} from "@app/services/ChatService.ts";
import {KeyboardEvent} from "react";
import {ChatMessage, TypingIndicator} from "@app/models/Chat.ts";
import {DateTime} from "luxon";
import NotificationService from "@app/utils/NotificationService.tsx"
import {ChatContext} from "@app/context/ChatContext.ts";
import {MessageTypes, MessageWithPayload} from "@app/models/Messages.ts";

type ChatProps = {
    activeChannelId: string | null;
}

function Chat({activeChannelId}: ChatProps) {
    const [name, setName] = useState("Mitchell");
    const [currentMessage, setCurrentMessage] = useState<string>("");
    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const scrollViewport = useRef<HTMLDivElement>(null);
    const [scrolledToBottom, setScrolledToBottom] = useState<boolean>(true);
    const [currentlyTypingUsers, setCurrentlyTypingUsers] = useState<Set<string>>(new Set([]));
    const [chatHasFocus, setChatHasFocus] = useState<boolean>(false);
    const [isTyping, setIsTyping] = useState<boolean>(false);

    const {chatService} = useContext(ChatContext);

    const send = () => {
        try {
            chatService.send(activeChannelId!, name, currentMessage);
            setCurrentMessage("");
        }
        catch (error: any) {
            console.error("Could not send message", error);
            NotificationService.showErrorMessage({
                title: "Could not send message",
                message: "Failed to send message: " + error.message,
            });
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

    useEffect(() => {
        if (activeChannelId === null) {
            return;
        }
        try {
            if (isTyping) {
                chatService.startTyping(activeChannelId, name)
            }
            else {
                chatService.stopTyping(activeChannelId, name)
            }
        }
        catch (error) {
            console.error("Could not send typing indicator: " + error);
        }

    }, [isTyping]);

    useEffect(() => {
        if (chatHasFocus) {
            setIsTyping(!StringUtils.isNullOrEmpty(currentMessage));
        }
    },[currentMessage]);

    // fetch any historical messages
    useEffect(() => {
        if (activeChannelId === null) {
            setMessages([]);
            return;
        }
        const fetch = async () => {
            try {
                const historicalMessages = await chatService.getHistoricalForChannel(activeChannelId);
                setMessages(historicalMessages)
            }
            catch (error: any) {
                NotificationService.showErrorMessage({
                    title: "Failed fetching messages",
                    message: "Could not connect to the server",
                });
            }
        };

        fetch()
            .catch(console.error);

    }, [activeChannelId]);

    // sign up for events from the WS
    useEffect(() => {
        const listener: MessageListener = {
            messageTypes: new Set([MessageTypes.ReceiveChatMessage, MessageTypes.StartedTyping, MessageTypes.StoppedTyping]),
            onMessage:(message) => {
                if (message.type === MessageTypes.ReceiveChatMessage) {
                    const chatMessage = (message as MessageWithPayload<ChatMessage>).payload;
                    if (chatMessage.channelId === activeChannelId) {
                        setMessages([...messages, chatMessage])
                    }
                }
                else if (message.type === MessageTypes.StartedTyping) {
                    const typingIndicator = (message as MessageWithPayload<TypingIndicator>).payload;
                    if (typingIndicator.channelId === activeChannelId) {
                        setCurrentlyTypingUsers(new Set([...currentlyTypingUsers, typingIndicator.user]));
                    }
                }
                else if (message.type === MessageTypes.StoppedTyping) {
                    const typingIndicator = (message as MessageWithPayload<TypingIndicator>).payload;
                    if (typingIndicator.channelId === activeChannelId) {
                        const newSet = new Set(currentlyTypingUsers);
                        newSet.delete(typingIndicator.user);
                        setCurrentlyTypingUsers(newSet);
                    }
                }
            }
        };

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

    useEffect(() => {
        setCurrentMessage("")
        setCurrentlyTypingUsers(new Set([]));
    }, [activeChannelId]);

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

    if (activeChannelId === null) {
        return (
            <Text>Please select a channel!</Text>
        );
    }

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
                            height: "calc(100vh - 240px)",
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
                {currentlyTypingUsers.size > 0 && (
                    <Text>{Array.from(currentlyTypingUsers.values()).join(" ,")} is currently typing...</Text>
                )}
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
                        onFocus={() => {
                            setChatHasFocus(true);
                            if (!StringUtils.isNullOrEmpty(currentMessage)) {
                                setIsTyping(true);
                            }
                        }}
                        onBlur={() => {
                            setChatHasFocus(false);
                            setIsTyping(false);
                        }}
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