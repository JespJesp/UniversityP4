using Ast;
using Ast.Nodes;
using Ast.Nodes.Timelines;
using Phases.Evaluation;

namespace UniversityP4.Tests;

[Trait("Category","Unit")]
public class EvaluatorTests
{
    [Fact]
    public void CascadeEvaluate_Should_Call_Evaluate_On_All_Nodes()
    {
        var programNode = CreateProgramNode(out var timelineNode);
        var childNode = new TrackingEvaluationNode();
        programNode.Children.Add(childNode);

        var evaluator = new Evaluator();
        evaluator.Evaluate(programNode, CreateFileInfo());

        childNode.EvaluateWasCalled.ShouldBeTrue();
    }

    [Fact]
    public void CascadeEvaluate_Should_Traverse_All_Children()
    {
        var programNode = CreateProgramNode(out var timelineNode);
        var child1 = new TrackingEvaluationNode();
        var child2 = new TrackingEvaluationNode();
        var grandChild = new TrackingEvaluationNode();
        
        programNode.Children.Add(child1);
        programNode.Children.Add(child2);
        child1.Children.Add(grandChild);

        var evaluator = new Evaluator();
        evaluator.Evaluate(programNode, CreateFileInfo());

        child1.EvaluateWasCalled.ShouldBeTrue();
        child2.EvaluateWasCalled.ShouldBeTrue();
        grandChild.EvaluateWasCalled.ShouldBeTrue();
    }

    [Fact]
    public void CascadeEvaluate_Should_Throw_When_Node_Evaluation_Fails()
    {
        var programNode = CreateProgramNode(out var timelineNode);
        var errorNode = new ErrorThrowingEvaluationNode();
        programNode.Children.Add(errorNode);

        var evaluator = new Evaluator();
        Action act = () => evaluator.Evaluate(programNode, CreateFileInfo());

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("Evaluation errors");
        exception.Message.ShouldContain("Evaluation failed");
    }

    [Fact]
    public void CascadeEvaluate_Should_Report_Node_Type_In_Error()
    {
        var programNode = CreateProgramNode(out var timelineNode);
        var errorNode = new ErrorThrowingEvaluationNode();
        programNode.Children.Add(errorNode);

        var evaluator = new Evaluator();
        Action act = () => evaluator.Evaluate(programNode, CreateFileInfo());

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain(nameof(ErrorThrowingEvaluationNode));
    }

    [Fact]
    public void CascadeEvaluate_Should_Report_Line_And_Column_In_Error()
    {
        var programNode = CreateProgramNode(out var timelineNode);
        var errorNode = new ErrorThrowingEvaluationNode { Location = new Location("file.mude", 25, 12) };
        programNode.Children.Add(errorNode);

        var evaluator = new Evaluator();
        Action act = () => evaluator.Evaluate(programNode, CreateFileInfo());

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("25");
        exception.Message.ShouldContain("12");
    }

    [Fact]
    public void CascadeEvaluate_Should_Accumulate_Multiple_Errors()
    {
        var programNode = CreateProgramNode(out var timelineNode);
        var errorNode1 = new ErrorThrowingEvaluationNode();
        var errorNode2 = new ErrorThrowingEvaluationNode();
        programNode.Children.Add(errorNode1);
        programNode.Children.Add(errorNode2);

        var evaluator = new Evaluator();
        Action act = () => evaluator.Evaluate(programNode, CreateFileInfo());

        var exception = Should.Throw<Exception>(act);
        (exception.Message.Split("Evaluation failed").Length - 1).ShouldBe(2);
    }

    [Fact]
    public void CascadeEvaluate_Should_Evaluate_Children_Before_Siblings()
    {
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

    private FileNode CreateProgramNode(out TimelineNode timelineNode)
    {
        var programNode = new FileNode { Location = new Location("file.mude", 1, 1) };
        timelineNode = new TimelineNode();
        TimelineNode.Instance = timelineNode;
        TimelineNode.InstanceCount = 1;

        var melodyNode = new Ast.Nodes.Melodies.MelodyNode
        {
            Id = "_lead",
            Melody = new Runtime.Objects.Melody
            {
                LengthInBeats = 1f,
                Notes =
                {
                    new Runtime.Objects.Note
                    {
                        StartBeat = 0f,
                        EndBeat = 1f,
                        Pitch = Runtime.Objects.Pitch.FromString("C4")
                    }
                },
                Samples =
                {
                    new Runtime.Objects.Sample
                    {
                        FilePath = "/ExamplePrograms/Samples/Drums/snare.wav"
                    }
                }
            }
        };

        timelineNode.SymbolTable.Upsert(melodyNode, melodyNode.Id);
        timelineNode.Timeline.Commands.Add(new Runtime.Objects.Timelines.TimelineCommand
        {
            Type = Runtime.Objects.Timelines.TimelineCommandType.Start,
            Beat = 0,
            TargetIds = new List<string> { "_lead" }
        });
        timelineNode.Timeline.Commands.Add(new Runtime.Objects.Timelines.TimelineCommand
        {
            Type = Runtime.Objects.Timelines.TimelineCommandType.Stop,
            Beat = 1,
            TargetIds = new List<string> { "_lead" }
        });

        programNode.Children.Add(timelineNode);

        return programNode;
    }

    private FileInfo CreateFileInfo()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current != null && !Directory.Exists(Path.Combine(current.FullName, "ExamplePrograms")))
        {
            current = current.Parent;
        }

        var projectRoot = current?.FullName ?? Directory.GetCurrentDirectory();
        return new FileInfo(Path.Combine(projectRoot, "UniversityP4.EvaluatorTests.wav"));
    }

    private class TrackingEvaluationNode : Node
    {
        public bool EvaluateWasCalled { get; private set; }

        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Evaluate(Evaluator evaluator)
        {
            EvaluateWasCalled = true;
        }
    }

    private class OrderTrackingEvaluationNode : Node
    {
        public List<string>? CallOrder { get; set; }
        public string? NodeName { get; set; }

        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Evaluate(Evaluator evaluator)
        {
            if (CallOrder != null && NodeName != null)
            {
                CallOrder.Add(NodeName);
            }
        }
    }

    private class ErrorThrowingEvaluationNode : Node
    {
        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Evaluate(Evaluator evaluator)
        {
            throw new Exception("Evaluation failed");
        }
    }

}
