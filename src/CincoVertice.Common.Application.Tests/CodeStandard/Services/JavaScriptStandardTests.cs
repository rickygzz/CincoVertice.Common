using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Services;

/// <summary>
///     The JavaScript code standard as the folder actions use it: both languages, each file by its extension.
/// </summary>
public class JavaScriptStandardTests : IDisposable
{
    // A Bruno script as it may come: 2 spaces and tabs, extra blank lines, //comment, spaces at line ends
    private const string Messy =
        "const axios = require('axios');\n"
        + "/**\n"
        + "   * Signs the request.\n"
        + "   */\n"
        + "function sign(req) {\n"
        + "  const body = req.getBody()  ;   \n"
        + "\tif (!body)\n"
        + "\t\treturn;\n"
        + "\n"
        + "\n"
        + "  const url = `https://x.com/${req.getUrl().replace(/\\/+$/, '')}?q=${q(\"a  b\")}`;\n"
        + "  const sql = `\n"
        + "  select  *   \n"
        + "  `;\n"
        + "  switch (req.getMethod()) {\n"
        + "  case 'GET':\n"
        + "    bru.setVar(\"m\", 'get');//no space\n"
        + "    break;\n"
        + "  }\n"
        + "  return axios.get(url)\n"
        + "  .then((res) => {\n"
        + "  return res.data;\n"
        + "\n"
        + "  });\n"
        + "}\n"
        + "module.exports = { sign };";

    private const string Fixed =
        "const axios = require('axios');\n"
        + "/**\n"
        + " * Signs the request.\n"
        + " */\n"
        + "function sign(req) {\n"
        + "    const body = req.getBody() ;\n"
        + "    if (!body)\n"
        + "        return;\n"
        + "\n"
        + "    const url = `https://x.com/${req.getUrl().replace(/\\/+$/, '')}?q=${q(\"a  b\")}`;\n"
        + "    const sql = `\n"
        + "  select  *   \n"
        + "  `;\n"
        + "    switch (req.getMethod()) {\n"
        + "        case 'GET':\n"
        + "            bru.setVar(\"m\", 'get');// no space\n"
        + "            break;\n"
        + "    }\n"
        + "    return axios.get(url)\n"
        + "        .then((res) => {\n"
        + "            return res.data;\n"
        + "        });\n"
        + "}\n"
        + "module.exports = { sign };\n";

    private readonly CodeCheckerService _checker = new(CodeLanguageModel.All);
    private readonly CodeFixerService _fixer = new(CodeLanguageModel.All);
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "JavaScriptStandardTests_" + Guid.NewGuid());

    public JavaScriptStandardTests()
    {
        Directory.CreateDirectory(_folder);
    }

    public void Dispose()
    {
        Directory.Delete(_folder, recursive: true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void CreateJavaScriptFormatting_FixesAScript_AndLeavesItsLiterals()
    {
        // Act
        string once = FixerPipeline.CreateJavaScriptFormatting().Run(Messy);
        string twice = FixerPipeline.CreateJavaScriptFormatting().Run(once);

        // Assert
        Assert.Equal(Fixed, once);
        Assert.Equal(once, twice);
    }

    [Fact]
    public void CreateJavaScriptFormatting_LeavesBracesOnTheLineOfTheirStatement()
    {
        // Arrange: on its own line, the { after return would make it return nothing
        string content = "function a() {\n    if (x) {\n        return {\n            b: 1,\n        };\n    } else {\n"
            + "        c();\n    }\n}\n";

        // Act & Assert
        Assert.Equal(content, FixerPipeline.CreateJavaScriptFormatting().Run(content));
    }

    [Fact]
    public void FixFolder_FixesJavaScriptFiles_SoTheCheckFindsNothing()
    {
        // Arrange
        string path = CreateFile("scripts/sign.js", Messy);
        int issuesBefore = _checker.CheckFile(path).Count;

        // Act
        var result = Assert.Single(_fixer.FixFolder(_folder));

        // Assert
        Assert.True(result.IsChanged);
        Assert.Equal(Fixed, File.ReadAllText(path));
        Assert.True(issuesBefore > 0);
        Assert.Empty(_checker.CheckFile(path));
    }

    [Fact]
    public void CheckFile_JavaScript_ReportsTheRulesForAnyLanguage()
    {
        // Arrange
        string path = CreateFile("a.js", "function a() {\n  b();  \n\n\n  c();//x\n}\n");

        // Act
        var codes = _checker.CheckFile(path).Select(error => (error.Line, error.Code)).ToList();

        // Assert
        Assert.Equal(
            [
                (2, nameof(Errors.CH0011)),
                (2, nameof(Errors.CH0036)),
                (4, nameof(Errors.CH0027)),
                (5, nameof(Errors.CH0036)),
                (5, nameof(Errors.CH0021)),
            ],
            codes);
    }

    [Theory]
    [InlineData("if (x) {\n    a();\n} else {\n    b();\n}\n")]
    [InlineData("const self = this" + ".value;\n")]
    [InlineData("const url = 'http://x.com';\nconst t = `\n  a  //b\n`;\n")]
    [InlineData("const t = `a  b`;\nconst re = /a  b/;\n")]
    public void CheckFile_JavaScript_LeavesOutTheCSharpRules_AndReadsItsLiterals(string content)
    {
        // Arrange: braces on the statement's line, this., // and double spaces inside literals
        string path = CreateFile("a.js", content);

        // Act & Assert
        Assert.Empty(_checker.CheckFile(path));
    }

    [Fact]
    public void CheckFile_CSharp_StillUsesTheCSharpRules()
    {
        // Arrange
        string path = CreateFile("a.cs", "public class A {\n}\n");

        // Act & Assert
        Assert.Contains(_checker.CheckFile(path), error => error.Code == nameof(Errors.CH0034));
    }

    [Fact]
    public void CheckFolder_TakesBothLanguages_AndSkipsPackagesAndMinifiedFiles()
    {
        // Arrange
        CreateFile("a.cs", "public class A\n{\n}\n");
        CreateFile("b.js", "a();\n");
        CreateFile("c.mjs", "a();\n");
        CreateFile("d.min.js", "a();\n");
        CreateFile("e.bru", "meta {\n}\n");
        CreateFile("f.csx", "a();\n");
        CreateFile(Path.Combine("node_modules", "lib", "g.js"), "a();\n");

        // Act
        var files = _checker.CheckFolder(_folder).Select(result => Path.GetFileName(result.FilePath));

        // Assert
        Assert.Equal(["a.cs", "b.js", "c.mjs"], files);
    }

    private string CreateFile(string relativePath, string content)
    {
        string path = Path.Combine(_folder, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);

        return path;
    }
}
