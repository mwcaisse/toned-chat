

const isNullOrEmpty = (s: string): boolean => {
    if (s === null || s === undefined) {
        return true;
    }
    return s.length === 0;
}

const isNullOrWhitespace = (s: string): boolean => {
    if (s === null || s === undefined) {
        return true;
    }

    return s.trim().length === 0;
}

export default {
    isNullOrWhitespace,
    isNullOrEmpty
}