using Song;

namespace SyntaxAnalysis.Parsing;

public static class ParseString
{
	public static void Parse(SyntaxAnalyzer a)
	{
		if (a.VisitableStack.Count == 0 || a.VisitableStack.Peek() is Pattern)
		{
			// This is a sample filename
			var sample = new Sample { FileName = a.CurrentToken.Value };
			a.CurrentPattern.Samples.Add(sample);
		}
	}
}