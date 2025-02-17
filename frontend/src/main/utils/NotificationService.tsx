import {notifications} from "@mantine/notifications";
import {IconX} from "@tabler/icons-react";


type NotificationParams = {
    title: string;
    message: string;
}

const showErrorMessage = (message: NotificationParams) => {
    notifications.show({
        position: "top-right",
        autoClose: 3000,
        color: "red",
        icon: <IconX />,
        title: message.title,
        message: message.message,
    });
}

export default {
    showErrorMessage
}