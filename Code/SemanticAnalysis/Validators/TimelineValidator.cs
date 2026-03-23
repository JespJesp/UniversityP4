using AST;

namespace SemanticAnalysis.Validators;

public static class TimelineValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Timeline timeline)
	{
		ValidateSettings(analyzer, timeline.Settings);
		ValidateCommands(analyzer, timeline.Commands);
	}

	private static void ValidateSettings(SemanticAnalyzer analyzer, TimelineSettings settings)
	{
		if (settings.Bpm <= 0)
		{
			analyzer.AddError($"BPM must be positive, got: {settings.Bpm}");
		}

		if (!settings.TimeSignature.Contains('/'))
		{
			analyzer.AddError($"Invalid time signature format: {settings.TimeSignature}");
		}
	}

	private static void ValidateCommands(SemanticAnalyzer analyzer, List<TimelineCommand> commands)
	{
		foreach (var command in commands)
		{
			ValidateCommand(analyzer, command);
		}
	}

	private static void ValidateCommand(SemanticAnalyzer analyzer, TimelineCommand command)
	{
		if (command.Beat.HasValue && command.Beat.Value <= 0)
		{
			analyzer.AddError($"Beat number must be positive, got: {command.Beat}");
		}

		if (command.Type == TimelineCommandType.Stop && command.PatternNames.Count == 0)
		{
			analyzer.AddError("Stop commands must specify pattern names or EVERYTHING");
		}
	}
}