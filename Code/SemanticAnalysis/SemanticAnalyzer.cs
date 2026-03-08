using System.Text.RegularExpressions;
using Visitation;
using Song;

namespace SemanticAnalysis;

public class SemanticAnalyzer : IVisitor
{
	private List<string> _errors = new List<string>();
	private Pattern _currentPattern;

	public void Analyze(Pattern pattern)
	{
		_errors.Clear();
		_currentPattern = pattern;
		pattern.Accept(this);

		if (_errors.Any())
			throw new Exception("Semantic errors:\n" + string.Join("\n", _errors));
	}

	public void VisitPattern(Pattern pattern)
	{
		if (pattern.Samples.Count == 0)
			_errors.Add("Pattern must have at least one sample");

		// Visit all samples
		foreach (var sample in pattern.Samples)
			sample.Accept(this);

		// Visit all notes
		foreach (var note in pattern.Notes)
			note.Accept(this);
	}

	public void VisitSample(Sample sample)
	{
		if (string.IsNullOrWhiteSpace(sample.FileName))
			_errors.Add("Sample filename cannot be empty");

		if (!sample.FileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
			_errors.Add($"Sample file {sample.FileName} should be a .wav file");
	}

	public void VisitNote(Note note)
	{
		if (note.StartTime < 0 || note.EndTime < 0)
			_errors.Add($"Note times must be positive: {note.StartTime}-{note.EndTime}");

		if (note.StartTime >= note.EndTime)
			_errors.Add($"Note start time must be less than end time: {note.StartTime}-{note.EndTime}");

		if (note.EndTime > _currentPattern.Length)
			_errors.Add($"Note end time {note.EndTime} exceeds pattern length {_currentPattern.Length}");

		// Validate note pitch format (simplified)
		if (!Regex.IsMatch(note.Pitch, @"^[a-g][0-9]$"))
			_errors.Add($"Invalid note pitch format: {note.Pitch}. Expected format like 'c5'");
	}
}