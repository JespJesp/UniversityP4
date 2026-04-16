using Ast.Tables;

namespace Runtime.Objects;

public class Timeline : RuntimeObject
{
	public int SampleRate = 48000;
	public int BeatsPerMinute = 120;
	public int BeatsPerBar = 4;
	public int BeatNoteValue = 4;

	public List<TimelineCommand> Commands = new();
	public List<Loop> Loops = new();

	public void BuildLoopsFromCommands(RuntimeVariableTable variables)
	{
		Loops.Clear();

		var activeMelodies = new Dictionary<Melody, List<ActiveLoopState>>();
		var commandTargets = new Dictionary<string, List<Melody>>();

		foreach (TimelineCommand command in Commands)
		{
			switch (command.Type)
			{
				case TimelineCommandType.Start:
					ExecuteStartCommand(command, activeMelodies, commandTargets, variables);
					break;
				case TimelineCommandType.Stop:
					ExecuteStopCommand(command, activeMelodies, commandTargets, variables);
					break;
				default:
					throw new Exception($"Unexpected timeline command type: {command.Type}");
			}
		}

		CloseOpenLoops(activeMelodies);
	}

	private void ExecuteStartCommand(
		TimelineCommand command,
		Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
		Dictionary<string, List<Melody>> commandTargets,
		RuntimeVariableTable variables)
	{
		float startBeat = command.Beat ?? 0;
		HashSet<Melody> melodies = ExpandTargetsToMelodies(command.TargetIds, variables);

		foreach (Melody melody in melodies)
		{
			if (!activeMelodies.TryGetValue(melody, out List<ActiveLoopState>? starts))
			{
				starts = new List<ActiveLoopState>();
				activeMelodies.Add(melody, starts);
			}

			starts.Add(new ActiveLoopState(startBeat, command.GainMultiplier, command.PitchShiftHalfsteps));
		}

		if (!string.IsNullOrWhiteSpace(command.Id))
		{
			commandTargets[command.Id] = melodies.ToList();
		}
	}

	private void ExecuteStopCommand(
		TimelineCommand command,
		Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
		Dictionary<string, List<Melody>> commandTargets,
		RuntimeVariableTable variables)
	{
		if (!command.Beat.HasValue)
		{
			throw new Exception("Timeline stop command is missing a beat value.");
		}

		float stopBeat = command.Beat.Value;
		HashSet<Melody> targetsToStop = ResolveStopTargets(command, activeMelodies, commandTargets, variables);

		foreach (Melody melody in targetsToStop)
		{
			if (!activeMelodies.TryGetValue(melody, out List<ActiveLoopState>? starts))
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
				Loops.Add(new Loop
				{
					Melody0 = adjustedMelody,
					StartBeat = start.StartBeat,
					EndBeat = stopBeat
				});
				starts.Remove(start);
			}

			if (starts.Count == 0)
			{
				activeMelodies.Remove(melody);
			}
		}

		if (!string.IsNullOrWhiteSpace(command.Id))
		{
			commandTargets.Remove(command.Id);
		}
	}

	private HashSet<Melody> ResolveStopTargets(
		TimelineCommand command,
		Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
		Dictionary<string, List<Melody>> commandTargets,
		RuntimeVariableTable variables)
	{
		HashSet<Melody> targetsToStop = new();

		if (command.TargetIds.Any(target => string.Equals(target, "EVERYTHING", StringComparison.OrdinalIgnoreCase)))
		{
			foreach (Melody melody in activeMelodies.Keys)
			{
				targetsToStop.Add(melody);
			}
			return targetsToStop;
		}

		if (command.TargetIds.Count > 0)
		{
			return ExpandTargetsToMelodies(command.TargetIds, variables);
		}

		if (!string.IsNullOrWhiteSpace(command.Id) && commandTargets.TryGetValue(command.Id, out List<Melody>? commandMelodies))
		{
			foreach (Melody melody in commandMelodies)
			{
				targetsToStop.Add(melody);
			}
		}

		return targetsToStop;
	}

	private void CloseOpenLoops(Dictionary<Melody, List<ActiveLoopState>> activeMelodies)
	{
		float timelineEndBeat = Commands
			.Where(command => command.Beat.HasValue)
			.Select(command => command.Beat!.Value)
			.DefaultIfEmpty(0)
			.Max();

		if (BeatsPerBar > 0)
		{
			timelineEndBeat = MathF.Ceiling(timelineEndBeat / BeatsPerBar) * BeatsPerBar;
		}

		foreach ((Melody melody, List<ActiveLoopState> starts) in activeMelodies)
		{
			foreach (ActiveLoopState start in starts)
			{
				float endBeat = Math.Max(timelineEndBeat, start.StartBeat + melody.LengthInBeats);
				if (endBeat > start.StartBeat)
				{
					Melody adjustedMelody = CreateAdjustedMelody(melody, start.GainMultiplier, start.PitchShiftHalfsteps);
					Loops.Add(new Loop
					{
						Melody0 = adjustedMelody,
						StartBeat = start.StartBeat,
						EndBeat = endBeat
					});
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
			LengthInBeats = source.LengthInBeats,
			Samples = new List<Sample>(source.Samples),
			Notes = new List<Note>()
		};

		foreach (Note sourceNote in source.Notes)
		{
			Note adjustedNote = new()
			{
				StartBeat = sourceNote.StartBeat,
				EndBeat = sourceNote.EndBeat,
				Volume = sourceNote.Volume * gainMultiplier,
				Pan = sourceNote.Pan,
				Pitch0 = ShiftPitch(sourceNote.Pitch0, pitchShiftHalfsteps),
				SampleOverride = sourceNote.SampleOverride
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

	private static HashSet<Melody> ExpandTargetsToMelodies(List<string> targets, RuntimeVariableTable variables)
	{
		HashSet<Melody> resolvedMelodies = new();

		foreach (string target in targets)
		{
			ResolveTargetToMelodies(target, resolvedMelodies, new HashSet<Pattern>(), variables);
		}

		return resolvedMelodies;
	}

	private static void ResolveTargetToMelodies(string targetId, HashSet<Melody> resolvedMelodies, HashSet<Pattern> visitedPatterns, RuntimeVariableTable variables)
	{
		if (variables.TryGet(targetId, out Melody melody))
		{
			resolvedMelodies.Add(melody);
			return;
		}

		if (!variables.TryGet(targetId, out Pattern pattern))
		{
			throw new Exception($"Timeline target '{targetId}' is undefined.");
		}

		if (!visitedPatterns.Add(pattern))
		{
			throw new Exception($"Timeline target '{targetId}' contains a recursive pattern reference.");
		}

		foreach (Melody childMelody in pattern.Melodies)
		{
			resolvedMelodies.Add(childMelody);
		}

		foreach (Pattern childPattern in pattern.Patterns)
		{
			ResolvePatternToMelodies(childPattern, resolvedMelodies, visitedPatterns);
		}

		visitedPatterns.Remove(pattern);
	}

	private static void ResolvePatternToMelodies(Pattern pattern, HashSet<Melody> resolvedMelodies, HashSet<Pattern> visitedPatterns)
	{
		if (!visitedPatterns.Add(pattern))
		{
			throw new Exception("Timeline target contains a recursive pattern reference.");
		}

		foreach (Melody melody in pattern.Melodies)
		{
			resolvedMelodies.Add(melody);
		}

		foreach (Pattern childPattern in pattern.Patterns)
		{
			ResolvePatternToMelodies(childPattern, resolvedMelodies, visitedPatterns);
		}

		visitedPatterns.Remove(pattern);
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