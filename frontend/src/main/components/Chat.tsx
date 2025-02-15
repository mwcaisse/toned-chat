import {Box, TextInput, Text, ScrollArea, Paper, Button, Group, Flex} from "@mantine/core";

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
                            <Button>Send</Button>
                        </Group>
                    )}
                />
            </Box>
        </Flex>

    )
}

export default Chat;