using System.Text.RegularExpressions;
using AST;

namespace SemanticAnalysis.Validators;

public static class NoteValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Note note)
	{
		if (note.StartTime < 0 || note.EndTime < 0)
		{
			analyzer.AddError($"Note times must be positive: {note.StartTime}-{note.EndTime}");
		}
		if (note.StartTime >= note.EndTime)
		{
			analyzer.AddError($"Note start time must be less than end time: {note.StartTime}-{note.EndTime}");
		}
		if (note.EndTime > note.ParentPattern.Length)
		{
			analyzer.AddError($"Note end time {note.EndTime} exceeds pattern length {note.ParentPattern.Length}");
		}
		if (!Regex.IsMatch(note.Pitch, @"^[a-g][0-9]$"))
		{
			analyzer.AddError($"Invalid note pitch format: {note.Pitch}. Expected format like 'c5'");
		}
	
		if (note.Volume < 0.0f || note.Volume > 1.0f)
      {
         analyzer.AddError($"Note volume must be between 0.0 and 1.0, but was: {note.Volume}");
      }
		if (note.Pan < -1.0f || note.Pan > 1.0f)
		{
    		analyzer.AddError($"Note pan must be between -100 and 100, but was: {(int)(note.Pan * 100)}");
		}
	}
}