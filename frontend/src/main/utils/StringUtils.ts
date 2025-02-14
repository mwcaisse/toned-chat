

const isNullOrEmpty = (s: string | null | undefined): boolean => {
    if (s === null || s === undefined) {
        return true;
    }
    return s.length === 0;
}

const isNullOrWhitespace = (s: string | null | undefined): boolean => {
    if (s === null || s === undefined) {
        return true;
    }

    return s.trim().length === 0;
}

export default {
    isNullOrWhitespace,
    isNullOrEmpty
}