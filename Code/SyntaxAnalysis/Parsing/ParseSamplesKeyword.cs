using Song;

namespace SyntaxAnalysis.Parsing;

public static class ParseSamplesKeyword
{
	public static void Parse(SyntaxAnalyzer a)
	{
		a.VisitableStack.Push(a.CurrentPattern);
	}
}