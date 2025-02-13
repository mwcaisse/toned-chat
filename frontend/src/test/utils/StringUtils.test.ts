import StringUtils from "@app/utils/StringUtils";


describe("isNullOrEmpty", () => {
    test("Should return true for empty string", () => {
        expect(StringUtils.isNullOrEmpty("")).toBe(true);
    })
});