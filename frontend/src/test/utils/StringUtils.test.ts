import StringUtils from "@app/utils/StringUtils";


describe("StringUtils", () => {
    describe("isNullOrEmpty", () => {
        test("Should return true for empty string", () => {
            expect(StringUtils.isNullOrEmpty("")).toBe(true);
        });

        test("Should return false for strings with values", () => {
            expect(StringUtils.isNullOrEmpty("hello world")).toBe(false);
            expect(StringUtils.isNullOrEmpty("bob")).toBe(false);
            expect(StringUtils.isNullOrEmpty("corn on the cob")).toBe(false);
        });

        test("Should return true for null / undefined", () => {
            expect(StringUtils.isNullOrEmpty(null)).toBe(true);
            expect(StringUtils.isNullOrEmpty(undefined)).toBe(true);
        });

        test("Should return false for all spaces", ()=> {
            expect(StringUtils.isNullOrEmpty("  ")).toBe(false);
            expect(StringUtils.isNullOrEmpty("    ")).toBe(false);
            expect(StringUtils.isNullOrEmpty("  \t  ")).toBe(false);
            expect(StringUtils.isNullOrEmpty("\t")).toBe(false);
        });
    });

    describe("isNullOrWhitespace", () => {
        test("Should return true for empty string", () => {
            expect(StringUtils.isNullOrWhitespace("")).toBe(true);
        });

        test("Should return true for all spaces", ()=> {
            expect(StringUtils.isNullOrWhitespace("  ")).toBe(true);
            expect(StringUtils.isNullOrWhitespace("    ")).toBe(true);
            expect(StringUtils.isNullOrWhitespace("  \t  ")).toBe(true);
            expect(StringUtils.isNullOrWhitespace("\t")).toBe(true);
        });

        test("Should return true for null / undefined", () => {
            expect(StringUtils.isNullOrWhitespace(null)).toBe(true);
            expect(StringUtils.isNullOrWhitespace(undefined)).toBe(true);
        });

        test("Should return false for strings with values", () => {
            expect(StringUtils.isNullOrWhitespace("hello world")).toBe(false);
            expect(StringUtils.isNullOrWhitespace("bob")).toBe(false);
            expect(StringUtils.isNullOrWhitespace("corn on the cob")).toBe(false);
        });
    })
});
