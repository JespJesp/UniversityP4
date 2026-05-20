using Ast;
using Ast.Nodes;
using Phases.Validation;

namespace UniversityP4.Tests;

[Trait("Category","Unit")]
public class ValidatorTests
{
    [Fact]
    public void Validate_Should_Call_Validate_On_All_Nodes()
    {
        var programNode = CreateProgramNode();
        var childNode = new TrackingValidationNode();
        programNode.Children.Add(childNode);

        var validator = new Validator();
        validator.Validate(programNode);

        childNode.ValidateWasCalled.ShouldBeTrue();
    }

    [Fact]
    public void Validate_Should_Traverse_All_Children()
    {
        var programNode = CreateProgramNode();
        var child1 = new TrackingValidationNode();
        var child2 = new TrackingValidationNode();
        var grandChild = new TrackingValidationNode();
        
        programNode.Children.Add(child1);
        programNode.Children.Add(child2);
        child1.Children.Add(grandChild);

        var validator = new Validator();
        validator.Validate(programNode);

        child1.ValidateWasCalled.ShouldBeTrue();
        child2.ValidateWasCalled.ShouldBeTrue();
        grandChild.ValidateWasCalled.ShouldBeTrue();
    }

    [Fact]
    public void Validate_Should_Throw_When_Node_Validation_Fails()
    {
        var programNode = CreateProgramNode();
        var errorNode = new ErrorThrowingValidationNode();
        programNode.Children.Add(errorNode);

        var validator = new Validator();
        Action act = () => validator.Validate(programNode);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("Semantic errors");
        exception.Message.ShouldContain("Validation failed");
    }

    [Fact]
    public void Validate_Should_Accumulate_Multiple_Errors()
    {
        var programNode = CreateProgramNode();
        var errorNode1 = new ErrorThrowingValidationNode();
        var errorNode2 = new ErrorThrowingValidationNode();
        programNode.Children.Add(errorNode1);
        programNode.Children.Add(errorNode2);

        var validator = new Validator();
        Action act = () => validator.Validate(programNode);

        var exception = Should.Throw<Exception>(act);
        var errorCount = exception.Message.Split("Validation failed").Length - 1;
        errorCount.ShouldBe(2);
    }

    [Fact]
    public void Validate_Should_Report_Node_Type_In_Error()
    {
        var programNode = CreateProgramNode();
        var errorNode = new ErrorThrowingValidationNode();
        programNode.Children.Add(errorNode);

        var validator = new Validator();
        Action act = () => validator.Validate(programNode);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain(nameof(ErrorThrowingValidationNode));
    }

    [Fact]
    public void Validate_Should_Report_Line_And_Column_In_Errors()
    {
        var programNode = CreateProgramNode();
        var errorNode = new ErrorThrowingValidationNode { Location = new Location("file.mude", 15, 8) };
        programNode.Children.Add(errorNode);

        var validator = new Validator();
        Action act = () => validator.Validate(programNode);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("15");
        exception.Message.ShouldContain("8");
    }

    [Fact]
    public void Validate_Should_Continue_After_Error_In_Child()
    {
        var programNode = CreateProgramNode();
        var errorNode = new ErrorThrowingValidationNode();
        var validNode = new TrackingValidationNode();
        programNode.Children.Add(errorNode);
        programNode.Children.Add(validNode);

        var validator = new Validator();
        Action act = () => validator.Validate(programNode);

        Should.Throw<Exception>(act);
        validNode.ValidateWasCalled.ShouldBeTrue();
    }

    private FileNode CreateProgramNode()
    {
        return new FileNode { Location = new Location("file.mude", 1, 1) };
    }

    private class TrackingValidationNode : Node
    {
        public bool ValidateWasCalled { get; private set; }

        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Validate(Validator validator)
        {
            ValidateWasCalled = true;
        }
    }

    private class ErrorThrowingValidationNode : Node
    {
        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Validate(Validator validator)
        {
            throw new Exception("Validation failed");
        }
    }
}
