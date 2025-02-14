import "@mantine/core/styles.css"

import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import App from "@app/App.tsx";
import {createTheme, MantineProvider} from "@mantine/core";

const theme = createTheme({});

createRoot(document.getElementById("root")!).render(
    <StrictMode>
        <MantineProvider theme={theme}>
            <App />
        </MantineProvider>
    </StrictMode>,
)
