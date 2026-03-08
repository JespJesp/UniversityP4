using Song;

namespace SyntaxAnalysis.Parsing;

public static class ParseIdentifier
{
	public static void Parse(SyntaxAnalyzer a)
	{
		if (a.CurrentToken.Value.Contains("_"))
		{
			// Pattern name with underscore
			var parts = a.CurrentToken.Value.Split('_');
			if (parts.Length == 2 && int.TryParse(parts[0], out _))
			{
				a.CurrentPattern.Name = parts[1];
			}
		}
		else if (a.VisitableStack.Peek() is Note)
		{
			// This is the note pitch
			var note = (Note)a.VisitableStack.Pop();
			note.Pitch = a.CurrentToken.Value;
			a.CurrentPattern.Notes.Add(note);
		}
	}
}