using Song;

namespace SyntaxAnalysis.Parsing;

public static class ParseHyphen
{
	public static void Parse(SyntaxAnalyzer a)
	{
		// Handle hyphen between note times
		if (a.VisitableStack.Peek() is Note note)
		{
			// Hyphen is processed when we get the end time
		}
	}
}