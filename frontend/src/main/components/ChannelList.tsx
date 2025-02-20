import {Channel} from "@app/models/Chat.ts";
import {NavLink} from "@mantine/core";
import {useContext, useEffect, useState} from "react";
import {ChatContext} from "@app/context/ChatContext.ts";
import NotificationService from "@app/utils/NotificationService.tsx";
import {IconPlus} from "@tabler/icons-react";
import CreateNewChannelModal from "@app/components/CreateNewChannelModal.tsx";

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
    }, [])


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
            {channels.map((channel: Channel) => (
                <NavLink
                    key={channel.id}
                    label={channel.name}
                    active={channel.id === activeChannelId}
                    onClick={() => clickChannelLink(channel)}
                />
            ))}

            <CreateNewChannelModal
                opened={createNewChannelModalOpened}
                close={() => setCreateNewChannelModalOpened(false)}
            />
        </>
    );
}