using Ast;
using Ast.Nodes;
using Ast.Nodes.Melodies;
using Ast.Nodes.Samples;

namespace UniversityP4.Tests;

[Trait("Category","Unit")]
public class SymbolTableTests
{
    [Fact]
    public void SymbolTable_Should_Initialize_Empty()
    {
        var table = new SymbolTable();

        table.Contains<MelodyNode>("lead").ShouldBeFalse();
    }

    [Fact]
    public void SymbolTable_Should_Store_Symbols()
    {
        var table = new SymbolTable();
        var melody = new MelodyNode { Id = "lead" };
        table.Upsert(melody, melody.Id);

        table.Contains<MelodyNode>("lead").ShouldBeTrue();
        table.Get<MelodyNode>("lead").ShouldBe(melody);
    }

    [Fact]
    public void SymbolTable_Should_Retrieve_Symbols_By_Type_And_Id()
    {
        var table = new SymbolTable();
        var melody = new MelodyNode { Id = "lead" };
        table.Upsert(melody, melody.Id);

        var retrieved = table.Get<MelodyNode>("lead");

        retrieved.ShouldBe(melody);
    }

    [Fact]
    public void SymbolTable_Should_Support_Multiple_Symbols_Of_Same_Type()
    {
        var table = new SymbolTable();
        var melody1 = new MelodyNode { Id = "lead" };
        var melody2 = new MelodyNode { Id = "harmony" };
        table.Upsert(melody1, melody1.Id);
        table.Upsert(melody2, melody2.Id);

        table.Contains<MelodyNode>("lead").ShouldBeTrue();
        table.Contains<MelodyNode>("harmony").ShouldBeTrue();
        table.Get<MelodyNode>("lead").ShouldBe(melody1);
        table.Get<MelodyNode>("harmony").ShouldBe(melody2);
    }

    [Fact]
    public void SymbolTable_Should_Return_False_For_NonExistent_Symbol()
    {
        var table = new SymbolTable();
        
        table.Contains<MelodyNode>("nonexistent").ShouldBeFalse();
    }

    [Fact]
    public void SymbolTable_Should_TryGet_Return_False_For_NonExistent()
    {
        var table = new SymbolTable();
        
        var success = table.TryGet<MelodyNode>("nonexistent", out var result);

        success.ShouldBeFalse();
    }

    [Fact]
    public void SymbolTable_Should_TryGet_Return_True_For_Existing()
    {
        var table = new SymbolTable();
        var melody = new MelodyNode { Id = "test" };
        table.Upsert(melody, melody.Id);

        var success = table.TryGet<MelodyNode>("test", out var result);

        success.ShouldBeTrue();
        result.ShouldBe(melody);
    }

    [Fact]
    public void SymbolTable_Clone_Should_Create_Independent_Copy()
    {
        var original = new SymbolTable();
        var melody = new MelodyNode { Id = "lead" };
        original.Upsert(melody, melody.Id);

        var cloned = original.Clone();

        cloned.Contains<MelodyNode>("lead").ShouldBeTrue();
        cloned.Get<MelodyNode>("lead").ShouldBe(melody);
    }

    [Fact]
    public void SymbolTable_Clone_Should_Not_Share_Dictionary()
    {
        var original = new SymbolTable();
        var melody1 = new MelodyNode { Id = "lead" };
        original.Upsert(melody1, melody1.Id);

        var cloned = original.Clone();
        
        var melody2 = new MelodyNode { Id = "harmony" };
        cloned.Upsert(melody2, melody2.Id);

        original.Contains<MelodyNode>("harmony").ShouldBeFalse();
        cloned.Contains<MelodyNode>("harmony").ShouldBeTrue();
    }

    [Fact]
    public void SymbolTable_Should_Throw_When_Getting_NonExistent_Symbol()
    {
        var table = new SymbolTable();

        Action act = () => table.Get<MelodyNode>("nonexistent");

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("cannot get symbol");
    }

    [Fact]
    public void SymbolTable_Should_Support_Large_Number_Of_Symbols()
    {
        var table = new SymbolTable();
        
        for (int i = 0; i < 100; i++)
        {
            var melody = new MelodyNode { Id = $"melody_{i}" };
            table.Upsert(melody, melody.Id);
        }

        table.Contains<MelodyNode>("melody_50").ShouldBeTrue();
    }
}
