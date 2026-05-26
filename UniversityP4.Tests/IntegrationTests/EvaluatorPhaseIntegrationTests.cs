using Ast;
using Phases.Evaluation;

namespace UniversityP4.Tests.IntegrationTests;

[Collection("Integration")]
[Trait("Category","Integration")]
public class EvaluatorPhaseIntegrationTests : IntegrationTestFixture
{
    [Fact]
    public void Evaluator_Should_Traverse_All_Nodes_In_Tree()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);
        var child1 = new TrackingEvaluationNode();
        var child2 = new TrackingEvaluationNode();

        programNode.Children.Add(child1);
        programNode.Children.Add(child2);

        var evaluator = new Evaluator();
        evaluator.Evaluate(programNode, CreateFileInfo());

        child1.EvaluateWasCalled.ShouldBeTrue();
        child2.EvaluateWasCalled.ShouldBeTrue();
    }

    [Fact]
    public void Evaluator_Should_Traverse_Nested_Children()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);
        var child = new TrackingEvaluationNode();
        var grandChild = new TrackingEvaluationNode();

        programNode.Children.Add(child);
        child.Children.Add(grandChild);

        var evaluator = new Evaluator();
        evaluator.Evaluate(programNode, CreateFileInfo());

        child.EvaluateWasCalled.ShouldBeTrue();
        grandChild.EvaluateWasCalled.ShouldBeTrue();
    }

    [Fact]
    public void Evaluator_Should_Respect_Evaluation_Order()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);
        var callOrder = new List<string>();

        var parent = new OrderTrackingEvaluationNode { CallOrder = callOrder, NodeName = "parent" };
        var child1 = new OrderTrackingEvaluationNode { CallOrder = callOrder, NodeName = "child1" };
        var child2 = new OrderTrackingEvaluationNode { CallOrder = callOrder, NodeName = "child2" };
        var sibling = new OrderTrackingEvaluationNode { CallOrder = callOrder, NodeName = "sibling" };

        programNode.Children.Add(parent);
        programNode.Children.Add(sibling);
        parent.Children.Add(child1);
        parent.Children.Add(child2);

        var evaluator = new Evaluator();
        evaluator.Evaluate(programNode, CreateFileInfo());

        callOrder.IndexOf("parent").ShouldBeLessThan(callOrder.IndexOf("child1"));
        callOrder.IndexOf("parent").ShouldBeLessThan(callOrder.IndexOf("child2"));
        callOrder.IndexOf("child2").ShouldBeLessThan(callOrder.IndexOf("sibling"));
    }

    [Fact]
    public void Evaluator_Should_Handle_Node_Evaluation_Errors()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);
        var errorNode = new ErrorThrowingEvaluationNode();

        programNode.Children.Add(errorNode);

        var evaluator = new Evaluator();
        Action act = () => evaluator.Evaluate(programNode, CreateFileInfo());

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("Evaluation failed");
    }

    [Fact]
    public void Evaluator_Should_Report_Error_Location()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);
        var errorNode = new ErrorThrowingEvaluationNode { Location = new Location("test.mude", 42, 15) };

        programNode.Children.Add(errorNode);

        var evaluator = new Evaluator();
        Action act = () => evaluator.Evaluate(programNode, CreateFileInfo());

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("42");
        exception.Message.ShouldContain("15");
    }

    [Fact]
    public void Evaluator_Should_Accumulate_Multiple_Errors()
    {
        ResetGlobalState();
        var programNode = CreateProgramNode(out var timelineNode);
        var errorNode1 = new ErrorThrowingEvaluationNode();
        var errorNode2 = new ErrorThrowingEvaluationNode();

        programNode.Children.Add(errorNode1);
        programNode.Children.Add(errorNode2);

        var evaluator = new Evaluator();
        Action act = () => evaluator.Evaluate(programNode, CreateFileInfo());

        var exception = Should.Throw<Exception>(act);
        var errorCount = exception.Message.Split("Evaluation failed").Length - 1;
        errorCount.ShouldBe(2);
    }

    [Fact]
    public void Evaluation_Should_Fail_For_Invalid_Sample_ReferencePitch()
    {
        ResetGlobalState();

        var source = string.Join("\n", new[]
        {
            "sample bad \"/ExamplePrograms/Samples/Instruments/electric guitar E4.wav\" z9",
            "melody 1 _test",
            "\tsamples",
            "\t\tbad",
            "\tchords",
            "\t\t0,1 c4",
            "\n",
            "timeline",
            "\tsettings",
            "\t\tbpm 120",
            "\t\ttimesignature 4,4",
            "\tstart 0",
            "\t\t1_test",
            "\tstop 1",
            "\t\t1_test",
        });

        var outputFile = CreateFileInfo("eval_invalid_sample_pitch.wav");

        try
        {
            DeleteIfExists(outputFile);
            Should.Throw<Exception>(() => RunFullPipeline(source, outputFile));
        }
        finally
        {
            DeleteIfExists(outputFile);
        }
    }

    [Fact]
    public void Evaluation_Should_Fail_When_Sample_File_Missing()
    {
        ResetGlobalState();

        var source = string.Join("\n", new[]
        {
            "sample ghost \"/ExamplePrograms/Samples/Nonexistent/ghost.wav\" c4",
            "melody 1 _test",
            "\tsamples",
            "\t\tghost",
            "\tchords",
            "\t\t0,1 c4",
            "\n",
            "timeline",
            "\tsettings",
            "\t\tbpm 120",
            "\t\ttimesignature 4,4",
            "\tstart 0",
            "\t\t1_test",
            "\tstop 1",
            "\t\t1_test",
        });

        var outputFile = CreateFileInfo("eval_missing_sample.wav");

        try
        {
            DeleteIfExists(outputFile);
            Should.Throw<Exception>(() => RunFullPipeline(source, outputFile));
        }
        finally
        {
            DeleteIfExists(outputFile);
        }
    }
}
