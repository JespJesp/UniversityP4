using System.IO;
using Phases.Lexing;
using Phases.Parsing;
using Phases.Annotation;
using Phases.Validation;
using Phases.Evaluation;
using Tokens;

namespace UniversityP4.Tests.IntegrationTests;

[Collection("Integration")]
public class IntegrationTests
{
    private static string RepoRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

    [Fact]
    public void FullPipeline_Should_Parse_Annotate_Validate_For_SimpleDrums()
    {
        var filePath = Path.Combine(RepoRoot, "ExamplePrograms", "Development.mude");
        var fileInfo = new FileInfo(filePath);
        var content = File.ReadAllText(filePath);

        Ast.Nodes.Timelines.TimelineNode.TimelineNodeInstances = 0;
        Ast.Nodes.Timelines.SettingsNode.SettingsNodeInstances = 0;

        var lexer = new Lexer();
        var tokens = lexer.Lex(content, fileInfo);

        var parser = new Parser();
        var program = parser.Parse(tokens);

        var annotator = new Annotator();
        annotator.Annotate(program);

        var validator = new Validator();
        validator.Validate(program);

        program.timelineNode.ShouldNotBeNull();
    }

    [Fact]
    public void FullPipeline_Should_Fail_On_Invalid_Sample_Error()
    {
        var filePath = Path.Combine(RepoRoot, "ExamplePrograms", "TimelineTest.mude");
        var fileInfo = new FileInfo(filePath);
        var content = File.ReadAllText(filePath);

        var lexer = new Lexer();
        var tokens = lexer.Lex(content, fileInfo);

        var parser = new Parser();
        var program = parser.Parse(tokens);

        var annotator = new Annotator();
        annotator.Annotate(program);

        var validator = new Validator();

        Should.Throw<Exception>(() => validator.Validate(program));
    }
}
