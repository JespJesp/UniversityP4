using Ast;
using Ast.Nodes;
using Phases.Annotation;

namespace UniversityP4.Tests;

public class AnnotatorTests
{
    [Fact]
    public void Annotate_Should_Create_SymbolTable_For_Root()
    {
        var programNode = CreateProgramNode();

        var annotator = new Annotator();
        annotator.Annotate(programNode);

        programNode.SymbolTable.ShouldNotBeNull();
    }

    [Fact]
    public void Annotate_Should_Propagate_SymbolTable_To_Children()
    {
        var programNode = CreateProgramNode();
        var childNode = new TestNode();
        programNode.Children.Add(childNode);

        var annotator = new Annotator();
        annotator.Annotate(programNode);

        childNode.SymbolTable.ShouldNotBeNull();
        childNode.SymbolTable.ShouldBeSameAs(programNode.SymbolTable);
    }

    [Fact]
    public void Annotate_Should_Call_Annotate_On_All_Nodes()
    {
        var programNode = CreateProgramNode();
        var childNode = new TrackingTestNode();
        programNode.Children.Add(childNode);

        var annotator = new Annotator();
        annotator.Annotate(programNode);

        childNode.AnnotateWasCalled.ShouldBeTrue();
    }

    [Fact]
    public void Annotate_Should_Accumulate_Errors_And_Throw()
    {
        var programNode = CreateProgramNode();
        var errorNode = new ErrorThrowingTestNode();
        programNode.Children.Add(errorNode);

        var annotator = new Annotator();
        Action act = () => annotator.Annotate(programNode);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("Semantic errors");
        exception.Message.ShouldContain("Test error");
    }

    [Fact]
    public void Annotate_Should_Report_Line_And_Column_In_Errors()
    {
        var programNode = CreateProgramNode();
        var errorNode = new ErrorThrowingTestNode { Location = new Location("file.mude", 42, 10) };
        programNode.Children.Add(errorNode);

        var annotator = new Annotator();
        Action act = () => annotator.Annotate(programNode);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("42");
        exception.Message.ShouldContain("10");
    }

    [Fact]
    public void Annotate_Should_Inherit_Symbols_From_Siblings()
    {
        var programNode = CreateProgramNode();
        var sibling1 = new TestNode { CreatesNestedScope = true };
        var sibling2 = new TestNode();
        programNode.Children.Add(sibling1);
        programNode.Children.Add(sibling2);

        var annotator = new Annotator();
        annotator.Annotate(programNode);

        sibling2.SymbolTable.ShouldNotBeNull();
    }

    private ProgramNode CreateProgramNode()
    {
        return new ProgramNode { Location = new Location("file.mude", 1, 1) };
    }

    private class TestNode : Node
    {
        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }
    }

    private class TrackingTestNode : Node
    {
        public bool AnnotateWasCalled { get; private set; }

        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Annotate(Annotator annotator)
        {
            AnnotateWasCalled = true;
        }
    }

    private class ErrorThrowingTestNode : Node
    {
        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Annotate(Annotator annotator)
        {
            throw new Exception("Test error from node");
        }
    }
}
