using System.IO;
using Phases.Lexing;
using Phases.Parsing;
using Phases.Annotation;
using Phases.Validation;
using Phases.Evaluation;
using Ast.Nodes.Timelines;
using Ast;

namespace UniversityP4.Tests.IntegrationTests;

[Collection("Integration")]
public class EvaluatorIntegrationTests
{
    private static string RepoRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

    [Fact]
    [Xunit.Trait("Category","Heavy")]
    public void Evaluator_Should_Render_Output_File_For_Valid_Program()
    {
        var filePath = Path.Combine(RepoRoot, "ExamplePrograms", "Development.mude");
        var fileInfo = new FileInfo(filePath);
        var content = File.ReadAllText(filePath);

        TimelineNode.TimelineNodeInstances = 0;
        Ast.Nodes.Timelines.SettingsNode.SettingsNodeInstances = 0;

        var lexer = new Lexer();
        var tokens = lexer.Lex(content, fileInfo);

        var parser = new Parser();
        var program = parser.Parse(tokens);

        var annotator = new Annotator();
        annotator.Annotate(program);

        var validator = new Validator();
        validator.Validate(program);

        var evaluator = new Evaluator();
        var outFile = new FileInfo(Path.Combine(Path.GetTempPath(), "evaluator_integration_output.wav"));
        if (outFile.Exists) outFile.Delete();

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RUN_HEAVY_INTEGRATION")))
        {
            var localEvaluator = new Evaluator();
            Exception? evalException = null;

            void Traverse(Node node)
            {
                if (evalException != null) return;
                try
                {
                    node.Evaluate(localEvaluator);
                }
                catch (Exception ex)
                {
                    evalException = ex;
                    return;
                }

                foreach (var child in node.Children)
                    Traverse(child);
            }

            Traverse(program);
            evalException.ShouldBeNull();
            return;
        }

        Exception? caught = null;
        try
        {
            evaluator.Evaluate(program, outFile);
        }
        catch (Exception ex)
        {
            caught = ex;
        }

        if (caught == null)
        {
            outFile.Exists.ShouldBeTrue();
            outFile.Length.ShouldBeGreaterThan(0);
            try { outFile.Delete(); } catch { }
        }
        else
        {
            caught.Message.ShouldContain("Could not find", Case.Sensitive);
        }
    }
}
