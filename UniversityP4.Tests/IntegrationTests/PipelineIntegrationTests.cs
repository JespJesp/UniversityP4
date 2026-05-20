using System.IO;
using Phases.Lexing;
using Phases.Parsing;
using Phases.Annotation;
using Phases.Validation;
using Phases.Evaluation;
using Ast.Nodes.Timelines;

namespace UniversityP4.Tests.IntegrationTests;

[Collection("Integration")]
[Trait("Category","Integration")]
public class PipelineIntegrationTests : IntegrationTestFixture
{
    [Fact]
    public void FullPipeline_Should_Lex_Parse_Annotate_Validate_For_Valid_Program()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);

        var evaluator = new Evaluator();
        evaluator.Evaluate(programNode, CreateFileInfo());

        timelineNode.ShouldNotBeNull();
    }

    [Fact]
    public void FullPipeline_Should_Parse_Valid_Simple_Melody_Program()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);

        programNode.ShouldNotBeNull();
        programNode.Children.ShouldNotBeEmpty();
    }

    [Fact]
    public void FullPipeline_Should_Annotate_Valid_Program()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);

        var annotator = new Phases.Annotation.Annotator();
        Should.NotThrow(() => annotator.Annotate(programNode));
    }

    [Fact]
    public void FullPipeline_Should_Validate_Valid_Program()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);

        var annotator = new Phases.Annotation.Annotator();
        annotator.Annotate(programNode);

        var validator = new Phases.Validation.Validator();
        Should.NotThrow(() => validator.Validate(programNode));
    }

    [Fact]
    public void Pipeline_Should_Evaluate_Complete_Program()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);
        var fileInfo = CreateFileInfo("pipeline_evaluation_test.wav");

        var evaluator = new Evaluator();
        Should.NotThrow(() => evaluator.Evaluate(programNode, fileInfo));
    }
}
