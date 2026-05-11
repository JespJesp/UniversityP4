using System.IO;
using System.Collections.Generic;
using Phases.Lexing;
using Phases.Parsing;
using Phases.Annotation;
using Phases.Validation;
using Ast.Nodes.Timelines;

namespace UniversityP4.Tests.IntegrationTests;

[Collection("Integration")]
public class NegativeFixturesTests
{
    private static string RepoRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

    public static IEnumerable<object[]> NegativeFiles()
    {
        var dir = Path.Combine(RepoRoot, "UniversityP4.Tests", "IntegrationTests", "TestPrograms", "Negative");
        if (!Directory.Exists(dir)) yield break;

        foreach (var file in Directory.EnumerateFiles(dir, "*.mude", SearchOption.TopDirectoryOnly))
            yield return new object[] { file };
    }

    [Xunit.Theory]
    [Xunit.MemberData(nameof(NegativeFiles))]
    public void NegativeFixture_Should_Fail_Pipeline(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        var content = File.ReadAllText(filePath);

        TimelineNode.TimelineNodeInstances = 0;
        Ast.Nodes.Timelines.SettingsNode.SettingsNodeInstances = 0;

        var lexer = new Lexer();
        var parser = new Parser();

        Should.Throw<Exception>(() =>
        {
            var tokens = lexer.Lex(content, fileInfo);
            var program = parser.Parse(tokens);

            var annotator = new Annotator();
            annotator.Annotate(program);

            var validator = new Validator();
            validator.Validate(program);
        });
    }
}
