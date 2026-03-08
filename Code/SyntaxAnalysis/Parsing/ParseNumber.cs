using Song;

namespace SyntaxAnalysis.Parsing;

public static class ParseNumber
{
	public static void Parse(SyntaxAnalyzer a)
	{
		if (a.VisitableStack.Count == 0)
		{
			// This is the pattern length
			a.CurrentPattern.Length = int.Parse(a.CurrentToken.Value);
		}
		else if (a.VisitableStack.Peek() is Pattern)
		{
			// This is a note start time
			var note = new Note();
			note.StartTime = int.Parse(a.CurrentToken.Value);
			a.VisitableStack.Push(note);
		}
	}
}