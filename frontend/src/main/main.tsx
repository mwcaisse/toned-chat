import "@mantine/core/styles.css"
import "@mantine/notifications/styles.css"

import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import App from "@app/App.tsx";
import {createTheme, MantineProvider} from "@mantine/core";
import {Notifications} from "@mantine/notifications";

const theme = createTheme({});

createRoot(document.getElementById("root")!).render(
    <StrictMode>
        <MantineProvider theme={theme}>
            <Notifications />
            <App />
        </MantineProvider>
    </StrictMode>,
)
