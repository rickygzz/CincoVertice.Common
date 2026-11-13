using CincoVertice.Common.Application.CodeStandard.Fixers.JavaScript;
using CincoVertice.Common.Application.CodeStandard.Helper;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers.JavaScript;

public class BraceIndentationFixerTests
{
    private readonly BraceIndentationFixer _fixer = new(4, JavaScriptCodeScanner.Instance);

    private static string Lines(params string[] lines) => string.Join("\n", lines) + "\n";

    private void AssertFixes(string expected, string content)
    {
        string once = _fixer.Fix(content);

        Assert.Equal(expected, once);
        Assert.Equal(once, _fixer.Fix(once));
    }

    [Fact]
    public void Fix_IndentsByBraces_WhateverTheIndentationWas()
    {
        AssertFixes(
            Lines("function a(x) {", "    if (x) {", "        b();", "    }", "}"),
            Lines("function a(x) {", "  if (x) {", "\t\tb();", "      }", "}"));
    }

    [Fact]
    public void Fix_BracketsOpenedOnOneLine_AreOneLevel()
    {
        AssertFixes(
            Lines("test('x', function () {", "    expect(a).to.equal({", "        b: [1, 2],", "    });", "});"),
            Lines("test('x', function () {", "expect(a).to.equal({", "b: [1, 2],", "});", "});"));
    }

    [Fact]
    public void Fix_ArraysAndObjects_OneItemPerLine()
    {
        AssertFixes(
            Lines("const a = {", "    list: [", "        1,", "        -1,", "    ],", "    b: 2,", "};"),
            Lines("const a = {", "  list: [", "    1,", "    -1,", "  ],", "    b: 2,", "};"));
    }

    [Fact]
    public void Fix_Switch_CasesOneLevelIn_TheirLinesTwo()
    {
        AssertFixes(
            Lines(
                "switch (x) {",
                "    case 1:",
                "        a();",
                "        break;",
                "    default: {",
                "        b();",
                "    }",
                "}"),
            Lines("switch (x) {", "case 1:", "a();", "break;", "default: {", "b();", "}", "}"));
    }

    [Fact]
    public void Fix_ChainedCalls_GoOneLevelIn()
    {
        AssertFixes(
            Lines(
                "return axios.get(url)",
                "    .then((res) => {",
                "        return res.data;",
                "    })",
                "    .catch(err =>",
                "        console.log(err));"),
            Lines(
                "return axios.get(url)",
                ".then((res) => {",
                "return res.data;",
                "})",
                ".catch(err =>",
                "console.log(err));"));
    }

    [Fact]
    public void Fix_AStatementGoingOnAfterAnOperator_GoesOneLevelIn()
    {
        AssertFixes(
            Lines("const total = a +", "    b +", "    c;", "const ok = x", "    && y;", "next();"),
            Lines("const total = a +", "b +", "c;", "const ok = x", "&& y;", "next();"));
    }

    [Fact]
    public void Fix_ABodyWithoutBraces_GoesOneLevelIn()
    {
        AssertFixes(
            Lines("if (!body)", "    return;", "else", "    go();", "for (const x of xs)", "    use(x);", "done();"),
            Lines("if (!body)", "return;", "else", "go();", "for (const x of xs)", "use(x);", "done();"));
    }

    [Fact]
    public void Fix_ACaseLabelOrIncrement_IsNotAContinuation()
    {
        AssertFixes(
            Lines("switch (x) {", "    case 1:", "        i++", "        j();", "}"),
            Lines("switch (x) {", "case 1:", "i++", "j();", "}"));
    }

    [Fact]
    public void Fix_LeavesTheLinesOfATemplateString()
    {
        AssertFixes(
            Lines("function q() {", "    const sql = `", "  select *", "     from x {", "`;", "    run(sql);", "}"),
            Lines("function q() {", "  const sql = `", "  select *", "     from x {", "`;", "  run(sql);", "}"));
    }

    [Fact]
    public void Fix_BracketsInLiteralsAndComments_DontCount()
    {
        AssertFixes(
            Lines("if (a) {", "    b('{', /[(]/, `}`); // {", "    /* ( */", "    c();", "}"),
            Lines("if (a) {", "b('{', /[(]/, `}`); // {", "/* ( */", "c();", "}"));
    }

    [Fact]
    public void Fix_LinesUpTheStarsOfADocComment()
    {
        AssertFixes(
            Lines("function a() {", "    /**", "     * Signs it.", "     * @param x", "     */", "    b();", "}"),
            Lines("function a() {", "/**", "   * Signs it.", "* @param x", "      */", "b();", "}"));
    }

    [Fact]
    public void Fix_OtherCommentLines_MoveWithTheirComment()
    {
        // The */ starts with *, so it goes under the first one
        AssertFixes(
            Lines("if (a) {", "    /*", "      table:", "        x  y", "     */", "}"),
            Lines("if (a) {", "/*", "  table:", "    x  y", "*/", "}"));
    }

    [Fact]
    public void Fix_KeepsBlankLinesAndLineEndings()
    {
        AssertFixes("if (a) {\r\n    b();\r\n\r\n   \r\n    c();\r\n}", "if (a) {\r\nb();\r\n\r\n   \r\nc();\r\n}");
    }

    [Fact]
    public void Fix_MoreClosingThanOpeningBrackets_StaysAtTheLeft()
    {
        AssertFixes(Lines("}", "a();", "});", "b();"), Lines("  }", "  a();", "});", "    b();"));
    }
}
