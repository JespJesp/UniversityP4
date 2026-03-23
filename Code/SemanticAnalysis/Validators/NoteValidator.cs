using System.Text.RegularExpressions;
using AbstractSyntax;

namespace SemanticAnalysis.Validators;

public static class NoteValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Note note)
	{
		// Time
		if (note.StartBeat < 0 || note.EndBeat < 0)
		{
			analyzer.AddError($"Note times must be positive: {note.StartBeat}-{note.EndBeat}");
		}
		if (note.StartBeat >= note.EndBeat)
		{
			analyzer.AddError($"Note start time must be less than end time: {note.StartBeat}-{note.EndBeat}");
		}
		if (note.EndBeat > note.ParentMelody.Length)
		{
			analyzer.AddError($"Note end time {note.EndBeat} exceeds melody length {note.ParentMelody.Length}");
		}
	}
}