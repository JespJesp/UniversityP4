using Ast;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;

namespace Runtime.Objects.Timeline;

internal static class TimelineTargetResolver
{
	public static HashSet<Melody> ExpandTargetsToMelodies(List<string> targets, SymbolTable globalSymbols)
	{
		HashSet<Melody> resolvedMelodies = new();

		foreach (string target in targets)
		{
			ResolveTargetToMelodies(target, resolvedMelodies, new HashSet<Pattern>(), globalSymbols);
		}

		return resolvedMelodies;
	}

	private static void ResolveTargetToMelodies(string targetId, HashSet<Melody> resolvedMelodies, HashSet<Pattern> visitedPatterns, SymbolTable globalSymbols)
	{
		if (globalSymbols.TryGet(targetId, out MelodyNode melodyNode))
		{
			resolvedMelodies.Add(melodyNode.Melody);
			return;
		}

		if (!globalSymbols.TryGet(targetId, out PatternNode patternNode))
		{
			throw new Exception($"Timeline target '{targetId}' is undefined.");
		}

		if (!visitedPatterns.Add(patternNode.Pattern))
		{
			throw new Exception($"Timeline target '{targetId}' contains a recursive pattern reference.");
		}

		foreach (Melody childMelody in patternNode.Pattern.Melodies)
		{
			resolvedMelodies.Add(childMelody);
		}

		foreach (Pattern childPattern in patternNode.Pattern.Patterns)
		{
			ResolvePatternToMelodies(childPattern, resolvedMelodies, visitedPatterns);
		}

		visitedPatterns.Remove(patternNode.Pattern);
	}

	private static void ResolvePatternToMelodies(Pattern pattern, HashSet<Melody> resolvedMelodies, HashSet<Pattern> visitedPatterns)
	{
		if (!visitedPatterns.Add(pattern))
		{
			throw new Exception("Timeline target contains a recursive pattern reference.");
		}

		foreach (Melody melody in pattern.Melodies)
		{
			resolvedMelodies.Add(melody);
		}

		foreach (Pattern childPattern in pattern.Patterns)
		{
			ResolvePatternToMelodies(childPattern, resolvedMelodies, visitedPatterns);
		}

		visitedPatterns.Remove(pattern);
	}
}
