import {Button, Group, Modal, TextInput} from "@mantine/core";
import {useForm} from "@mantine/form";
import StringUtils from "@app/utils/StringUtils.ts";
import {useContext} from "react";
import {ChatContext} from "@app/context/ChatContext.ts";
import NotificationService from "@app/utils/NotificationService.tsx";


type CreateNewChannelModalProps = {
    opened: boolean;
    close: () => void;
}

const validChannelNameRegex = new RegExp(/^[A-Za-z0-9_\-]+$/);

export default function CreateNewChannelModal({opened, close}: CreateNewChannelModalProps) {

    const {chatService} = useContext(ChatContext);

    const form = useForm({
        mode: "uncontrolled",
        initialValues: {
            name: ""
        },
        validate: {
            name: (value) => {
                // TODO: We should disallow other values, but yolo for now
                const isValid = !StringUtils.isNullOrWhitespace(value) && validChannelNameRegex.test(value);
                return isValid ? null : "Invalid channel name. Can only contain alphanumeric characters and - or _"
            }
        }
    });

    const submit = () => {
        form.validate();
        const isValid = form.isValid();
        if (!isValid) {
            return;
        }
        try {
            chatService.createChannel(form.getValues().name);
            close();
            form.reset();
        }
        catch (error: any) {
            console.error("Could not create channel", error);
            NotificationService.showErrorMessage({
                title: "Could create channel",
                message: "Failed to create channel: " + error.message,
            });
        }
    };

    return (
        <Modal
            opened={opened}
            onClose={close}
            title="Create New Channel"
            centered
        >
            <TextInput
                withAsterisk
                label="Channel Name"
                key={form.key["name"]}
                {...form.getInputProps("name")}
            />
            <Group justify="flex-end" mt="md">
                <Button onClick={submit}>Create</Button>
            </Group>
        </Modal>
    );
}
