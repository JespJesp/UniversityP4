using Ast;
using Ast.Nodes;
using Ast.Nodes.Floats;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;
using Ast.Nodes.Samples;
using Ast.Nodes.Strings;
using Ast.Nodes.Timelines;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;

namespace UniversityP4.Tests;

public class ProgramNodeTests
{
    [Fact]
    public void ProgramNode_Should_Initialize_With_TimelineNode()
    {
        var programNode = new ProgramNode();

        programNode.timelineNode.ShouldNotBeNull();
        programNode.timelineNode.ShouldBeOfType<TimelineNode>();
    }

    [Fact]
    public void ProgramNode_Should_Accept_Children()
    {
        var programNode = new ProgramNode();
        var childNode = new MelodyNode();

        programNode.Children.Add(childNode);

        programNode.Children.Count.ShouldBe(1);
        programNode.Children[0].ShouldBe(childNode);
    }

    [Fact]
    public void ProgramNode_Should_Support_Multiple_Children()
    {
        var programNode = new ProgramNode();
        programNode.Children.Add(new MelodyNode());
        programNode.Children.Add(new PatternNode());
        programNode.Children.Add(new SampleNode());

        programNode.Children.Count.ShouldBe(3);
    }
}

public class FloatConstantNodeTests
{
    [Fact]
    public void FloatConstantNode_Should_Initialize()
    {
        var node = new FloatConstantNode();

        node.ShouldNotBeNull();
    }

    [Fact]
    public void FloatConstantNode_Should_Store_Id()
    {
        var node = new FloatConstantNode { Id = "my_float" };

        node.Id.ShouldBe("my_float");
    }

    [Fact]
    public void FloatConstantNode_Is_A_SymbolNode()
    {
        var node = new FloatConstantNode { Id = "test" };

        node.ShouldBeAssignableTo<SymbolNode>();
    }
}

public class StringConstantNodeTests
{
    [Fact]
    public void StringConstantNode_Should_Initialize()
    {
        var node = new StringConstantNode();

        node.ShouldNotBeNull();
    }

    [Fact]
    public void StringConstantNode_Should_Store_Id()
    {
        var node = new StringConstantNode { Id = "my_string" };

        node.Id.ShouldBe("my_string");
    }

    [Fact]
    public void StringConstantNode_Is_A_SymbolNode()
    {
        var node = new StringConstantNode { Id = "test" };

        node.ShouldBeAssignableTo<SymbolNode>();
    }
}

public class SymbolNodeTests
{
    [Fact]
    public void SymbolNode_Should_Store_Id()
    {
        var node = new TestSymbolNode { Id = "test_id" };

        node.Id.ShouldBe("test_id");
    }

    [Fact]
    public void SymbolNode_Should_Support_Different_Ids()
    {
        var node1 = new TestSymbolNode { Id = "id1" };
        var node2 = new TestSymbolNode { Id = "id2" };

        node1.Id.ShouldNotBe(node2.Id);
    }

    [Fact]
    public void SymbolNode_Should_Be_Registrable_In_Symbol_Table()
    {
        var node = new TestSymbolNode { Id = "melody_id" };
        var symbolTable = new SymbolTable();

        // SymbolTable should be able to store symbol nodes
        // This tests that the SymbolNode can be used with SymbolTable
        node.SymbolTable = symbolTable;
        node.SymbolTable.ShouldBe(symbolTable);
    }

    private class TestSymbolNode : SymbolNode
    {
        public override void CascadeParse(Parser parser)
        {
            throw new NotImplementedException();
        }
    }
}

public class MelodyNodeTests
{
    [Fact]
    public void MelodyNode_Should_Initialize_With_Empty_Melody()
    {
        var node = new MelodyNode();

        node.Melody.ShouldNotBeNull();
        node.Melody.Notes.ShouldBeEmpty();
        node.Melody.Samples.ShouldBeEmpty();
    }

    [Fact]
    public void MelodyNode_Should_Store_LengthInBeats()
    {
        var node = new MelodyNode { LengthInBeats = 4.0f };

        node.LengthInBeats.ShouldBe(4.0f);
    }

    [Fact]
    public void MelodyNode_Should_Be_Able_To_Generate_Id()
    {
        var node = new MelodyNode { LengthInBeats = 2.0f };
        node.Id = node.LengthInBeats + "test";

        node.Id.ShouldBe("2test");
    }
}

public class PatternNodeTests
{
    [Fact]
    public void PatternNode_Should_Initialize_With_Empty_Pattern()
    {
        var node = new PatternNode();

        node.Pattern.ShouldNotBeNull();
        node.Pattern.Melodies.ShouldBeEmpty();
        node.Pattern.Patterns.ShouldBeEmpty();
    }

    [Fact]
    public void PatternNode_Should_Store_LengthInBeats()
    {
        var node = new PatternNode { LengthInBeats = 8.0f };

        node.LengthInBeats.ShouldBe(8.0f);
    }

    [Fact]
    public void PatternNode_Should_Support_Nested_Patterns()
    {
        var parent = new PatternNode();
        var child = new PatternNode();
        parent.Children.Add(child);

        parent.Children.Count.ShouldBe(1);
        parent.Children[0].ShouldBe(child);
    }
}

public class SampleNodeTests
{
    [Fact]
    public void SampleNode_Should_Initialize_With_Empty_Sample()
    {
        var node = new SampleNode();

        node.Sample.ShouldNotBeNull();
        node.Sample.FilePath.ShouldBe("");
    }

    [Fact]
    public void SampleNode_Should_Store_Reference_Pitch()
    {
        var pitch = new Pitch { PitchClass = 0, Octave = 4 };
        var node = new SampleNode();
        node.Sample.ReferencePitch = pitch;

        node.Sample.ReferencePitch.PitchClass.ShouldBe(0);
        node.Sample.ReferencePitch.Octave.ShouldBe(4);
    }

    [Fact]
    public void SampleNode_Should_Store_ADSR_Parameters()
    {
        var node = new SampleNode();
        node.Sample.AttackBeats = 0.1f;
        node.Sample.DecayBeats = 0.2f;
        node.Sample.ReleaseBeats = 0.3f;

        node.Sample.AttackBeats.ShouldBe(0.1f);
        node.Sample.DecayBeats.ShouldBe(0.2f);
        node.Sample.ReleaseBeats.ShouldBe(0.3f);
    }
}

public class TimelineNodeTests
{
    [Fact]
    public void TimelineNode_Should_Initialize()
    {
        var node = new TimelineNode();

        node.ShouldNotBeNull();
    }

    [Fact]
    public void TimelineNode_Should_Accept_Children()
    {
        var node = new TimelineNode();
        var child = new SettingsNode(node);
        node.Children.Add(child);

        node.Children.Count.ShouldBe(1);
    }
}
