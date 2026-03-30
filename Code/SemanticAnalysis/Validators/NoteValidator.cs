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
		if (note.EndBeat > note.ParentMelody.LengthInBeats)
		{
			analyzer.AddError($"Note end time {note.EndBeat} exceeds melody length {note.ParentMelody.LengthInBeats}");
		}

		if (note.Volume < 0.0f)
		{
			analyzer.AddError($"Note volume cannot be negative, but was: {note.Volume}");
		}
		if (note.Pan < -1.0f || note.Pan > 1.0f)
		{
			analyzer.AddError($"Note pan must be between -1 and 1, but was: {note.Pan}");
		}
	}
}