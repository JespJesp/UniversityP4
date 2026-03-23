using System.Text.RegularExpressions;
using AbstractSyntax;

namespace SemanticAnalysis.Validators;

public static class NoteValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Note note)
	{
		// Time
		if (note.StartTime < 0 || note.EndTime < 0)
		{
			analyzer.AddError($"Note times must be positive: {note.StartTime}-{note.EndTime}");
		}
		if (note.StartTime >= note.EndTime)
		{
			analyzer.AddError($"Note start time must be less than end time: {note.StartTime}-{note.EndTime}");
		}
		if (note.EndTime > note.ParentMelody.Length)
		{
			analyzer.AddError($"Note end time {note.EndTime} exceeds melody length {note.ParentMelody.Length}");
		}
	}
}