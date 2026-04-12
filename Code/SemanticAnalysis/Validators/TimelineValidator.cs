using AbstractSyntax;

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

		if (settings.TimeSignatureNumerator <= 0 || settings.TimeSignatureDenominator <= 0)
		{
			analyzer.AddError($"Time signature values must be positive, got: {settings.TimeSignatureNumerator}/{settings.TimeSignatureDenominator}");
		}
	}

	private static void ValidateCommands(SemanticAnalyzer analyzer, List<TimelineCommand> commands)
	{
		foreach (TimelineCommand command in commands)
		{
			ValidateCommand(analyzer, command);
		}
	}

	private static void ValidateCommand(SemanticAnalyzer analyzer, TimelineCommand command)
	{
		if (command.Beat.HasValue && command.Beat.Value < 0)
		{
			analyzer.AddError($"Beat number must be positive, got: {command.Beat}");
		}

		if (command.Type == TimelineCommandType.Start && command.TargetIds.Count == 0)
		{
			analyzer.AddError("Start commands must specify at least one target melody or pattern");
		}
		if (command.Type == TimelineCommandType.Start && command.GainMultiplier < 0)
		{
			analyzer.AddError($"Timeline gain cannot be negative, got: {command.GainMultiplier}");
		}

		if (command.Type == TimelineCommandType.Stop && !command.Beat.HasValue)
		{
			analyzer.AddError("Stop commands must specify a beat value");
		}
		if (command.Type == TimelineCommandType.Stop && (command.GainMultiplier != 1.0f || command.PitchShiftHalfsteps != 0.0f))
		{
			analyzer.AddError("Stop commands cannot use gain or pitch modifiers");
		}

		if (command.Type == TimelineCommandType.Stop && command.TargetIds.Count == 0 && string.IsNullOrWhiteSpace(command.Id))
		{
			analyzer.AddError("Stop commands must specify targets, EVERYTHING, or a command ID");
		}

		foreach (string targetId in command.TargetIds)
		{
			if (string.Equals(targetId, "EVERYTHING", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			if (!RuntimeEnvironment.Melodies.ContainsKey(targetId) && !RuntimeEnvironment.Patterns.ContainsKey(targetId))
			{
				analyzer.AddError($"Timeline target '{targetId}' is undefined.");
			}
		}
	}
}