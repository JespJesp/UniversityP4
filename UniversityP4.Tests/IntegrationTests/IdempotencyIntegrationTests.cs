using System.IO;
using Phases.Lexing;
using Phases.Parsing;
using Phases.Annotation;
using Phases.Validation;
using Ast.Nodes.Timelines;

namespace UniversityP4.Tests.IntegrationTests;

[Collection("Integration")]
public class IdempotencyIntegrationTests
{
    private static string RepoRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

    [Fact]
    public void Annotate_Validate_Can_Run_Multiple_Times_Idempotently()
    {
        var filePath = Path.Combine(RepoRoot, "ExamplePrograms", "Development.mude");
        var fileInfo = new FileInfo(filePath);
        var content = File.ReadAllText(filePath);

        for (int i = 0; i < 2; i++)
        {
            TimelineNode.TimelineNodeInstances = 0;
            Ast.Nodes.Timelines.SettingsNode.SettingsNodeInstances = 0;

            var lexer = new Lexer();
            var tokens = lexer.Lex(content, fileInfo);

            var parser = new Parser();
            var program = parser.Parse(tokens);

            var annotator = new Annotator();
            var validator = new Validator();

            Should.NotThrow(() => annotator.Annotate(program));
            Should.NotThrow(() => validator.Validate(program));
        }
    }
}
