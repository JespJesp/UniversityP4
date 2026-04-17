using Ast.Tables;

namespace Runtime.Objects;

internal static class TimelineLoopBuilder
{
	public static void Build(Timeline timeline, RuntimeVariableTable variables)
	{
		timeline.Loops.Clear();

		var activeMelodies = new Dictionary<Melody, List<ActiveLoopState>>();
		var commandTargets = new Dictionary<string, List<Melody>>();

		foreach (TimelineCommand command in timeline.Commands)
		{
			switch (command.Type)
			{
				case TimelineCommandType.Start:
					ExecuteStartCommand(command, activeMelodies, commandTargets, variables);
					break;
				case TimelineCommandType.Stop:
					ExecuteStopCommand(command, timeline.Loops, activeMelodies, commandTargets, variables);
					break;
				default:
					throw new Exception($"Unexpected timeline command type: {command.Type}");
			}
		}

		CloseOpenLoops(timeline, activeMelodies);
	}

	private static void ExecuteStartCommand(
		TimelineCommand command,
		Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
		Dictionary<string, List<Melody>> commandTargets,
		RuntimeVariableTable variables)
	{
		float startBeat = command.Beat ?? 0;
		HashSet<Melody> melodies = TimelineTargetResolver.ExpandTargetsToMelodies(command.TargetIds, variables);

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

	private static void ExecuteStopCommand(
		TimelineCommand command,
		List<Loop> loops,
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

				Melody adjustedMelody = TimelineMelodyTransformer.CreateAdjustedMelody(melody, start.GainMultiplier, start.PitchShiftHalfsteps);
				loops.Add(new Loop
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

	private static HashSet<Melody> ResolveStopTargets(
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
			return TimelineTargetResolver.ExpandTargetsToMelodies(command.TargetIds, variables);
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

	private static void CloseOpenLoops(Timeline timeline, Dictionary<Melody, List<ActiveLoopState>> activeMelodies)
	{
		float timelineEndBeat = timeline.Commands
			.Where(command => command.Beat.HasValue)
			.Select(command => command.Beat!.Value)
			.DefaultIfEmpty(0)
			.Max();

		if (timeline.BeatsPerBar > 0)
		{
			timelineEndBeat = MathF.Ceiling(timelineEndBeat / timeline.BeatsPerBar) * timeline.BeatsPerBar;
		}

		foreach ((Melody melody, List<ActiveLoopState> starts) in activeMelodies)
		{
			foreach (ActiveLoopState start in starts)
			{
				float endBeat = Math.Max(timelineEndBeat, start.StartBeat + melody.LengthInBeats);
				if (endBeat > start.StartBeat)
				{
					Melody adjustedMelody = TimelineMelodyTransformer.CreateAdjustedMelody(melody, start.GainMultiplier, start.PitchShiftHalfsteps);
					timeline.Loops.Add(new Loop
					{
						Melody0 = adjustedMelody,
						StartBeat = start.StartBeat,
						EndBeat = endBeat
					});
				}
			}
		}
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
