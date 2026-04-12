namespace AbstractSyntax;

public class Timeline
{
	public TimelineSettings Settings = new();
	public List<TimelineCommand> Commands = new();
	public List<Loop> Loops = new();

	public void BuildLoopsFromCommands()
	{
		Loops.Clear();

		RuntimeEnvironment.BeatsPerMinute = Settings.Bpm;
		RuntimeEnvironment.BeatsPerBar = Settings.TimeSignatureNumerator;
		RuntimeEnvironment.BeatNoteValue = Settings.TimeSignatureDenominator;

		var activeMelodies = new Dictionary<string, List<ActiveLoopState>>();
		var commandTargets = new Dictionary<string, List<string>>();

		foreach (TimelineCommand command in Commands)
		{
			switch (command.Type)
			{
				case TimelineCommandType.Start:
					ExecuteStartCommand(command, activeMelodies, commandTargets);
					break;
				case TimelineCommandType.Stop:
					ExecuteStopCommand(command, activeMelodies, commandTargets);
					break;
				default:
					throw new Exception($"Unexpected timeline command type: {command.Type}");
			}
		}

		CloseOpenLoops(activeMelodies);
	}

	private void ExecuteStartCommand(
		TimelineCommand command,
		Dictionary<string, List<ActiveLoopState>> activeMelodies,
		Dictionary<string, List<string>> commandTargets)
	{
		float startBeat = command.Beat ?? 0;
		HashSet<string> melodyIds = ExpandTargetsToMelodies(command.TargetIds);

		foreach (string melodyId in melodyIds)
		{
			if (!activeMelodies.TryGetValue(melodyId, out List<ActiveLoopState>? starts))
			{
				starts = new List<ActiveLoopState>();
				activeMelodies.Add(melodyId, starts);
			}

			starts.Add(new ActiveLoopState(startBeat, command.GainMultiplier, command.PitchShiftHalfsteps));
		}

		if (!string.IsNullOrWhiteSpace(command.Id))
		{
			commandTargets[command.Id] = melodyIds.ToList();
		}
	}

	private void ExecuteStopCommand(
		TimelineCommand command,
		Dictionary<string, List<ActiveLoopState>> activeMelodies,
		Dictionary<string, List<string>> commandTargets)
	{
		if (!command.Beat.HasValue)
		{
			throw new Exception("Timeline stop command is missing a beat value.");
		}

		float stopBeat = command.Beat.Value;
		HashSet<string> targetsToStop = ResolveStopTargets(command, activeMelodies, commandTargets);

		foreach (string melodyId in targetsToStop)
		{
			if (!activeMelodies.TryGetValue(melodyId, out List<ActiveLoopState>? starts))
			{
				continue;
			}

			if (!RuntimeEnvironment.Melodies.TryGetValue(melodyId, out Melody? melody))
			{
				continue;
			}

			foreach (ActiveLoopState start in starts.ToList())
			{
				if (start.StartBeat >= stopBeat)
				{
					continue;
				}

				Melody adjustedMelody = CreateAdjustedMelody(melody, start.GainMultiplier, start.PitchShiftHalfsteps);
				Loops.Add(new Loop(adjustedMelody, start.StartBeat, stopBeat));
				starts.Remove(start);
			}

			if (starts.Count == 0)
			{
				activeMelodies.Remove(melodyId);
			}
		}

		if (!string.IsNullOrWhiteSpace(command.Id))
		{
			commandTargets.Remove(command.Id);
		}
	}

	private static HashSet<string> ResolveStopTargets(
		TimelineCommand command,
		Dictionary<string, List<ActiveLoopState>> activeMelodies,
		Dictionary<string, List<string>> commandTargets)
	{
		HashSet<string> targetsToStop = new();

		if (command.TargetIds.Any(target => string.Equals(target, "EVERYTHING", StringComparison.OrdinalIgnoreCase)))
		{
			foreach (string melodyId in activeMelodies.Keys)
			{
				targetsToStop.Add(melodyId);
			}
			return targetsToStop;
		}

		if (command.TargetIds.Count > 0)
		{
			return ExpandTargetsToMelodies(command.TargetIds);
		}

		if (!string.IsNullOrWhiteSpace(command.Id) && commandTargets.TryGetValue(command.Id, out List<string>? commandMelodies))
		{
			foreach (string melodyId in commandMelodies)
			{
				targetsToStop.Add(melodyId);
			}
		}

		return targetsToStop;
	}

	private void CloseOpenLoops(Dictionary<string, List<ActiveLoopState>> activeMelodies)
	{
		float timelineEndBeat = Commands
			.Where(command => command.Beat.HasValue)
			.Select(command => command.Beat!.Value)
			.DefaultIfEmpty(0)
			.Max();

		if (RuntimeEnvironment.BeatsPerBar > 0)
		{
			timelineEndBeat = MathF.Ceiling(timelineEndBeat / RuntimeEnvironment.BeatsPerBar) * RuntimeEnvironment.BeatsPerBar;
		}

		foreach ((string melodyId, List<ActiveLoopState> starts) in activeMelodies)
		{
			if (!RuntimeEnvironment.Melodies.TryGetValue(melodyId, out Melody? melody))
			{
				continue;
			}

			foreach (ActiveLoopState start in starts)
			{
				float endBeat = Math.Max(timelineEndBeat, start.StartBeat + melody.LengthInBeats);
				if (endBeat > start.StartBeat)
				{
					Melody adjustedMelody = CreateAdjustedMelody(melody, start.GainMultiplier, start.PitchShiftHalfsteps);
					Loops.Add(new Loop(adjustedMelody, start.StartBeat, endBeat));
				}
			}
		}
	}

	private static Melody CreateAdjustedMelody(Melody source, float gainMultiplier, float pitchShiftHalfsteps)
	{
		if (gainMultiplier == 1.0f && pitchShiftHalfsteps == 0.0f)
		{
			return source;
		}

		Melody adjustedMelody = new()
		{
			Id = source.Id,
			LengthInBeats = source.LengthInBeats,
			SampleIds = new List<string>(source.SampleIds),
			Notes = new List<Note>()
		};

		foreach (Note sourceNote in source.Notes)
		{
			Note adjustedNote = new(adjustedMelody)
			{
				StartBeat = sourceNote.StartBeat,
				EndBeat = sourceNote.EndBeat,
				Volume = sourceNote.Volume * gainMultiplier,
				Pan = sourceNote.Pan,
				ThePitch = ShiftPitch(sourceNote.ThePitch, pitchShiftHalfsteps)
			};

			adjustedMelody.Notes.Add(adjustedNote);
		}

		return adjustedMelody;
	}

	private static Pitch ShiftPitch(Pitch sourcePitch, float pitchShiftHalfsteps)
	{
		int roundedHalfsteps = (int)MathF.Round(pitchShiftHalfsteps);
		int absoluteHalfsteps = sourcePitch.Octave * 12 + sourcePitch.PitchClass + roundedHalfsteps;
		if (absoluteHalfsteps < 0)
		{
			absoluteHalfsteps = 0;
		}

		Pitch shiftedPitch = new("c0")
		{
			Octave = absoluteHalfsteps / 12,
			PitchClass = absoluteHalfsteps % 12
		};

		return shiftedPitch;
	}

	private static HashSet<string> ExpandTargetsToMelodies(List<string> targets)
	{
		HashSet<string> resolvedMelodies = new();

		foreach (string target in targets)
		{
			ResolveTargetToMelodies(target, resolvedMelodies, new HashSet<string>());
		}

		return resolvedMelodies;
	}

	private static void ResolveTargetToMelodies(string targetId, HashSet<string> resolvedMelodies, HashSet<string> visitedPatterns)
	{
		if (RuntimeEnvironment.Melodies.ContainsKey(targetId))
		{
			resolvedMelodies.Add(targetId);
			return;
		}

		if (!RuntimeEnvironment.Patterns.TryGetValue(targetId, out Pattern? pattern))
		{
			throw new Exception($"Timeline target '{targetId}' is undefined.");
		}

		if (!visitedPatterns.Add(targetId))
		{
			throw new Exception($"Timeline target '{targetId}' contains a recursive pattern reference.");
		}

		foreach (string childId in pattern.PatternAndMelodyIds)
		{
			ResolveTargetToMelodies(childId, resolvedMelodies, visitedPatterns);
		}

		visitedPatterns.Remove(targetId);
	}

	public void Reset()
	{
		Settings = new TimelineSettings();
		Commands.Clear();
		Loops.Clear();
	}

	private sealed class ActiveLoopState
	{
		public float StartBeat { get; }
		public float GainMultiplier { get; }
		public float PitchShiftHalfsteps { get; }

		public ActiveLoopState(float startBeat, float gainMultiplier, float pitchShiftHalfsteps)
		{
			StartBeat = startBeat;
			GainMultiplier = gainMultiplier;
			PitchShiftHalfsteps = pitchShiftHalfsteps;
		}
	}
}

public class TimelineSettings
{
	public int Bpm = 120;
	public int TimeSignatureNumerator = 4;
	public int TimeSignatureDenominator = 4;
}

public class TimelineCommand
{
	public string Id = "";
	public TimelineCommandType Type;
	public float? Beat;
	public List<string> TargetIds = new();
	public float GainMultiplier = 1.0f;
	public float PitchShiftHalfsteps = 0.0f;
}

public enum TimelineCommandType
{
	Start,
	Stop
}
