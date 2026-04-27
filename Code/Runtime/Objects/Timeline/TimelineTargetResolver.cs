using Ast.Tables;

namespace Runtime.Objects;

internal static class TimelineTargetResolver
{
	public static HashSet<Melody> ExpandTargetsToMelodies(List<string> targets, RuntimeVariableTable variables)
	{
		HashSet<Melody> resolvedMelodies = new();

		foreach (string target in targets)
		{
			ResolveTargetToMelodies(target, resolvedMelodies, new HashSet<Pattern>(), variables);
		}

		return resolvedMelodies;
	}

	private static void ResolveTargetToMelodies(string targetId, HashSet<Melody> resolvedMelodies, HashSet<Pattern> visitedPatterns, RuntimeVariableTable variables)
	{
		if (variables.TryGet(targetId, out Melody melody))
		{
			resolvedMelodies.Add(melody);
			return;
		}

		if (!variables.TryGet(targetId, out Pattern pattern))
		{
			throw new Exception($"Timeline target '{targetId}' is undefined.");
		}

		if (!visitedPatterns.Add(pattern))
		{
			throw new Exception($"Timeline target '{targetId}' contains a recursive pattern reference.");
		}

		foreach (Melody childMelody in pattern.Melodies)
		{
			resolvedMelodies.Add(childMelody);
		}

		foreach (Pattern childPattern in pattern.Patterns)
		{
			ResolvePatternToMelodies(childPattern, resolvedMelodies, visitedPatterns);
		}

		visitedPatterns.Remove(pattern);
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
