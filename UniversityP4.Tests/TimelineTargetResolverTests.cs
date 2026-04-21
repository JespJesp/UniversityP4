using System.Reflection;
using System.Runtime.ExceptionServices;
using Ast.Tables;
using Runtime.Objects;

namespace UniversityP4.Tests;

public class TimelineTargetResolverTests
{
    [Fact]
    public void ExpandTargetsToMelodies_Should_Resolve_Direct_Melody_Target()
    {
        var melody = new Melody();
        var variables = CreateVariables(
            ((typeof(Melody), "_lead"), melody));

        var resolved = InvokeResolver(new List<string> { "_lead" }, variables);

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

        var variables = CreateVariables(
            ((typeof(Pattern), "_song"), rootPattern));

        var resolved = InvokeResolver(new List<string> { "_song", "_song" }, variables);

        resolved.Count.ShouldBe(2);
        resolved.ShouldContain(melodyA);
        resolved.ShouldContain(melodyB);
    }

    [Fact]
    public void ExpandTargetsToMelodies_Should_Throw_When_Target_Is_Undefined()
    {
        var variables = new RuntimeVariableTable();

        Action act = () => InvokeResolver(new List<string> { "_missing" }, variables);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("Timeline target '_missing' is undefined.");
    }

    [Fact]
    public void ExpandTargetsToMelodies_Should_Throw_When_Pattern_Recursion_Is_Detected()
    {
        var recursivePattern = new Pattern();
        recursivePattern.Patterns.Add(recursivePattern);

        var variables = CreateVariables(
            ((typeof(Pattern), "_loop"), recursivePattern));

        Action act = () => InvokeResolver(new List<string> { "_loop" }, variables);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("recursive pattern reference");
    }

    private static RuntimeVariableTable CreateVariables(params ((Type type, string id) key, RuntimeObject value)[] entries)
    {
        var table = new RuntimeVariableTable();
        var dictionary = new Dictionary<(Type, string), RuntimeObject>();

        foreach (var (key, value) in entries)
        {
            dictionary[key] = value;
        }

        typeof(RuntimeVariableTable)
            .GetField("_variables", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(table, dictionary);

        return table;
    }

    private static HashSet<Melody> InvokeResolver(List<string> targets, RuntimeVariableTable variables)
    {
        var resolverType = typeof(Timeline).Assembly.GetType("Runtime.Objects.TimelineTargetResolver", throwOnError: true)!;
        var method = resolverType.GetMethod("ExpandTargetsToMelodies", BindingFlags.Public | BindingFlags.Static)!;

        try
        {
            return (HashSet<Melody>)method.Invoke(null, new object[] { targets, variables })!;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }
}
