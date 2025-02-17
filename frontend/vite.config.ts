import { defineConfig } from "vite"
import react from "@vitejs/plugin-react-swc"
import path from "path"

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    resolve: {
        alias: {
            "@app": path.resolve(__dirname, "src/main/"),
            "@test": path.resolve(__dirname, "src/test/"),
            '@tabler/icons-react': '@tabler/icons-react/dist/esm/icons/index.mjs',
        }
    }
})
