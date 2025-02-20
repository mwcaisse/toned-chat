import {Channel, ChatMessage} from "@app/models/Chat.ts";
import {NavLink} from "@mantine/core";
import {useContext, useEffect, useMemo, useState} from "react";
import {ChatContext} from "@app/context/ChatContext.ts";
import NotificationService from "@app/utils/NotificationService.tsx";
import {IconPlus} from "@tabler/icons-react";
import CreateNewChannelModal from "@app/components/CreateNewChannelModal.tsx";
import {MessageListener} from "@app/services/ChatService.ts";
import { MessageTypes, MessageWithPayload } from "@app/models/Messages";

type ChannelListProps = {
    activeChannelId: string | null;
    setActiveChannelId: (activeChannelId: string) => void;
}

export default function ChannelList(
    {activeChannelId, setActiveChannelId}: ChannelListProps
) {
    const {chatService} = useContext(ChatContext);
    const [channels, setChannels] = useState<Channel[]>([]);
    const [createNewChannelModalOpened, setCreateNewChannelModalOpened] = useState(false);
    const [channelsWithNewMessages, setChannelsWithNewMessages] = useState<{[channelId: string]: boolean}>({});

    useEffect(() => {
        const fetch = async () => {
            try {
                const res = await chatService.getChannels();
                setChannels(res);

                // if there are any channels, set the first one as the active one
                if (res.length > 0) {
                    setActiveChannelId(res[0].id);
                }
            }
            catch (error: any) {
                NotificationService.showErrorMessage({
                    title: "Failed fetching channels",
                    message: "Could not connect to the server",
                });
            }
        };

        fetch()
            .catch(console.error);
    }, []);

    // sign up for events from the WS
    useEffect(() => {
        const listener: MessageListener = {
            messageTypes: new Set([MessageTypes.ChannelCreated, MessageTypes.ReceiveChatMessage]),
            onMessage:(message) => {
                if (message.type === MessageTypes.ChannelCreated) {
                    const channel = (message as MessageWithPayload<Channel>).payload;
                    const newChannels = [...channels, channel];
                    setChannels(newChannels.sort((l, r) => {
                        return l.name.localeCompare(r.name);
                    }));
                }
                else if (message.type === MessageTypes.ReceiveChatMessage) {
                    const chatMessage = (message as MessageWithPayload<ChatMessage>).payload;
                    if (chatMessage.channelId !== activeChannelId) {
                        setChannelsWithNewMessages({...channelsWithNewMessages, [chatMessage.channelId]: true});
                    }
                }
            }
        };

        chatService.addListener(listener);
        return () => {
            chatService.removeListener(listener)
        }
    });

    useMemo(() => {
        if (activeChannelId !== null) {
            setChannelsWithNewMessages({...channelsWithNewMessages, [activeChannelId]: false});
        }
    }, [activeChannelId])


    const clickChannelLink = (channel: Channel) => {
        setActiveChannelId(channel.id);
    };

    const clickCreateNewChannel = () => {
        setCreateNewChannelModalOpened(true);
    };



    return (
        <>
            <NavLink
                label={"Create Channel"}
                leftSection={<IconPlus />}
                onClick={clickCreateNewChannel}
            />
            {channels.map((channel: Channel) => {
                let props = {}
                if (activeChannelId === channel.id) {
                    props = {
                        active: true,
                        variant: "filled"
                    };
                }
                else if (channelsWithNewMessages[channel.id]) {
                    props = {
                        active: true,
                        variant: "subtle",
                    };
                }

                return (
                    <NavLink
                        key={channel.id}
                        label={channel.name}
                        onClick={() => clickChannelLink(channel)}
                        {...props}
                    />
                )
            })}
            <CreateNewChannelModal
                opened={createNewChannelModalOpened}
                close={() => setCreateNewChannelModalOpened(false)}
            />
        </>
    );
}
