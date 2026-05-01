using System.Reflection;
using System.Runtime.ExceptionServices;
using Ast;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;
using Runtime.AudioRendering.Loops;
using Runtime.Objects;

namespace UniversityP4.Tests;

public class TimelineTargetResolverTests
{
    [Fact]
    public void ExpandTargetsToMelodies_Should_Resolve_Direct_Melody_Target()
    {
        var melody = new Melody();
        var symbols = CreateSymbols(
            (typeof(MelodyNode), "_lead", CreateMelodyNode("_lead", melody)));

        var resolved = InvokeResolver(new List<string> { "_lead" }, symbols);

        resolved.Count.ShouldBe(1);
        resolved.Single().ShouldBeSameAs(melody);
    }

    [Fact]
    public void ExpandTargetsToMelodies_Should_Resolve_Nested_Pattern_Targets_Without_Duplicates()
    {
        var melodyA = new Melody();
        var melodyB = new Melody();

        var childPattern = new Pattern();
        childPattern.Melodies.Add(melodyB);

        var rootPattern = new Pattern();
        rootPattern.Melodies.Add(melodyA);
        rootPattern.Patterns.Add(childPattern);

        var symbols = CreateSymbols(
            (typeof(PatternNode), "_song", CreatePatternNode("_song", rootPattern)));

        var resolved = InvokeResolver(new List<string> { "_song", "_song" }, symbols);

        resolved.Count.ShouldBe(2);
        resolved.ShouldContain(melodyA);
        resolved.ShouldContain(melodyB);
    }

    [Fact]
    public void ExpandTargetsToMelodies_Should_Throw_When_Target_Is_Undefined()
    {
        var symbols = new SymbolTable();

        Action act = () => InvokeResolver(new List<string> { "_missing" }, symbols);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("Timeline target '_missing' is undefined");
    }

    [Fact]
    public void ExpandTargetsToMelodies_Should_Throw_When_Pattern_Recursion_Is_Detected()
    {
        var recursivePattern = new Pattern();
        recursivePattern.Patterns.Add(recursivePattern);

        var symbols = CreateSymbols(
            (typeof(PatternNode), "_loop", CreatePatternNode("_loop", recursivePattern)));

        Action act = () => InvokeResolver(new List<string> { "_loop" }, symbols);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("recursive pattern reference");
    }

    private static MelodyNode CreateMelodyNode(string id, Melody melody)
    {
        return new MelodyNode
        {
            Id = id,
            Melody = melody
        };
    }

    private static PatternNode CreatePatternNode(string id, Pattern pattern)
    {
        return new PatternNode
        {
            Id = id,
            Pattern = pattern
        };
    }

    private static SymbolTable CreateSymbols(params (Type type, string id, SymbolNode node)[] entries)
    {
        var table = new SymbolTable();

        foreach (var (type, id, node) in entries)
        {
            table.Upsert(node, id);
        }

        return table;
    }

    private static HashSet<Melody> InvokeResolver(List<string> targets, SymbolTable symbols)
    {
        var resolverType = typeof(LoopBuilder).Assembly.GetType("Runtime.AudioRendering.Loops.TimelineCommandTargetResolver", throwOnError: true)!;
        var method = resolverType.GetMethod("ExpandTargetsToMelodies", BindingFlags.Public | BindingFlags.Static)!;

        try
        {
            return (HashSet<Melody>)method.Invoke(null, new object[] { targets, symbols })!;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }
}
