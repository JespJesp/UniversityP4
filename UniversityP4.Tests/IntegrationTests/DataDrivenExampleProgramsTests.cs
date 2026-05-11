using System.IO;
using System.Collections.Generic;
using System.Linq;
using Phases.Lexing;
using Phases.Parsing;
using Phases.Annotation;
using Phases.Validation;
using Ast.Nodes.Timelines;

namespace UniversityP4.Tests.IntegrationTests;

[Collection("Integration")]
public class DataDrivenExampleProgramsTests
{
    private static string RepoRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

    public static IEnumerable<object[]> ExampleFiles()
    {
        var testProgramDir = Path.Combine(RepoRoot, "UniversityP4.Tests", "IntegrationTests", "TestPrograms");
        if (Directory.Exists(testProgramDir))
        {
            foreach (var file in Directory.EnumerateFiles(testProgramDir, "*.mude", SearchOption.TopDirectoryOnly))
                yield return new object[] { file };

            yield break;
        }

        var dir = Path.Combine(RepoRoot, "ExamplePrograms");
        if (!Directory.Exists(dir)) yield break;

        foreach (var file in Directory.EnumerateFiles(dir, "*.mude", SearchOption.TopDirectoryOnly))
        {
            var name = Path.GetFileName(file);
            if (string.Equals(name, "Development2.mude", StringComparison.OrdinalIgnoreCase))
                continue;

            yield return new object[] { file };
        }
    }

    [Xunit.Theory]
    [Xunit.MemberData(nameof(ExampleFiles))]
    public void ExampleProgram_Pipeline_Should_Pass_For_TopLevel_Files(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        var content = File.ReadAllText(filePath);

        TimelineNode.TimelineNodeInstances = 0;
        Ast.Nodes.Timelines.SettingsNode.SettingsNodeInstances = 0;

        var lexer = new Lexer();
        var tokens = lexer.Lex(content, fileInfo);

        var parser = new Parser();
        var program = parser.Parse(tokens);

        var annotator = new Annotator();
        var validator = new Validator();

        Exception? caught = null;
        try
        {
            annotator.Annotate(program);
            validator.Validate(program);
        }
        catch (Exception ex)
        {
            caught = ex;
        }

        caught.ShouldBeNull($"Expected example program to validate: {filePath} (got: {caught?.Message})");
    }
}
