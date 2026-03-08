using Song;

namespace SyntaxAnalysis.Parsing;

public static class ParseSample
{
	public static void Parse(SyntaxAnalyzer a)
	{
		a.VisitableStack.Push(a.CurrentPattern);
	}
}