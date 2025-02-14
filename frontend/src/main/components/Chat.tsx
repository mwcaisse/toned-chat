import {Box, TextInput, Text, ScrollArea, Paper} from "@mantine/core";
import classes from "@app/components/Chat.module.css";

type Message = {
    user: string;
    message: string;
}

function Chat() {

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

    return (
        <Box h="1100px">
            <ScrollArea h="100%">
                {messages.map((message) =>
                    <Paper shadow="xs" p="xs" m="xs">
                        <Text size="xs">{message.user}</Text>
                        <Text size="md">{message.message}</Text>
                    </Paper>
                )}
            </ScrollArea>

            <Box className={classes.inputHolder}>
                <TextInput
                    label="Chat"
                />
            </Box>
        </Box>

    )
}

export default Chat;