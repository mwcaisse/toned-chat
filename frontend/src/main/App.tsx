
import {Button, Center, Container, TextInput} from "@mantine/core";

function App() {

    return (
        <>
            <Container>
                <Center>
                    <p>Welcome to Toned Chat!</p>
                </Center>
                <Center>
                    What shall we call you?
                </Center>
                <Center>
                    <TextInput
                        placeholder="Name"
                    />
                    <Button variant="filled">Get Started</Button>
                </Center>
            </Container>
        </>
    )
}

export default App
