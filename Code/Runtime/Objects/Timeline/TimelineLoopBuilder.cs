using Ast.Tables;

namespace Runtime.Objects;

internal static class TimelineLoopBuilder
{
	public static void Build(Timeline timeline, RuntimeVariableTable variables)
	{
		timeline.Loops.Clear();

		var activeMelodies = new Dictionary<Melody, List<ActiveLoopState>>();
		var commandTargets = new Dictionary<string, List<Melody>>();
		float currentBeat = 0;

		foreach (TimelineCommand command in timeline.Commands)
		{
			switch (command.Type)
			{
				case TimelineCommandType.Start:
					ExecuteStartCommand(command, activeMelodies, commandTargets, globalSymbols, ref currentBeat);
					break;
				case TimelineCommandType.Stop:
					ExecuteStopCommand(command, Loops, activeMelodies, commandTargets, globalSymbols, ref currentBeat);
					break;
				default:
					throw new Exception($"Unexpected timeline command type: {command.Type}");
			}
		}

		CloseOpenLoops(timeline, activeMelodies, currentBeat);

		return Loops;
	}

	private void ExecuteStartCommand(
			TimelineCommand command,
			Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
			Dictionary<string, List<Melody>> commandTargets,
			SymbolTable globalSymbols,
			ref float currentBeat)
	{
		float startBeat = command.Beat ?? currentBeat;
		HashSet<Melody> melodies = TimelineCommandTargetResolver.ExpandTargetsToMelodies(command.TargetIds, globalSymbols);

		foreach (Melody melody in melodies)
		{
			if (!activeMelodies.TryGetValue(melody, out List<ActiveLoopState>? starts))
			{
				starts = new List<ActiveLoopState>();
				activeMelodies.Add(melody, starts);
			}

			starts.Add(new ActiveLoopState(startBeat, command.GainMultiplier, command.PitchShiftHalfsteps, command.PanOffset));
		}

		if (!string.IsNullOrWhiteSpace(command.Id))
		{
			commandTargets[command.Id] = melodies.ToList();
		}

		currentBeat = startBeat;
	}

	private void ExecuteStopCommand(
			TimelineCommand command,
			List<Loop> loops,
			Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
			Dictionary<string, List<Melody>> commandTargets,
			SymbolTable globalSymbols,
			ref float currentBeat)
	{
		if (!command.Beat.HasValue)
		{
			throw new Exception("Timeline stop command is missing a beat value.");
		}

		float stopBeat = currentBeat + command.Beat.Value;
		HashSet<Melody> targetsToStop = ResolveStopTargets(command, activeMelodies, commandTargets, globalSymbols);

		foreach (Melody melody in targetsToStop)
		{
			if (!activeMelodies.TryGetValue(melody, out List<ActiveLoopState>? starts))
			{
				continue;
			}

			foreach (ActiveLoopState start in starts.ToList())
			{
				float endBeat = stopBeat;
				if (endBeat <= start.StartBeat)
				{
					continue;
				}

				Melody adjustedMelody = MelodyModifier.CreateAdjustedMelody(melody, start.GainMultiplier, start.PitchShiftHalfsteps, start.PanOffset);
				loops.Add(new Loop
				{
					Melody0 = adjustedMelody,
					StartBeat = start.StartBeat,
					EndBeat = endBeat
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

		currentBeat = stopBeat;
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

	private void CloseOpenLoops(Timeline timeline, Dictionary<Melody, List<ActiveLoopState>> activeMelodies, float currentBeat)
	{
		float timelineEndBeat = currentBeat;

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
					Melody adjustedMelody = MelodyModifier.CreateAdjustedMelody(melody, start.GainMultiplier, start.PitchShiftHalfsteps, start.PanOffset);
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

	private sealed class ActiveLoopState
	{
		public float StartBeat { get; }
		public float GainMultiplier { get; }
		public float PitchShiftHalfsteps { get; }
		public float PanOffset { get; }

		public ActiveLoopState(float startBeat, float gainMultiplier, float pitchShiftHalfsteps, float panOffset)
		{
			StartBeat = startBeat;
			GainMultiplier = gainMultiplier;
			PitchShiftHalfsteps = pitchShiftHalfsteps;
			PanOffset = panOffset;
		}
	}
}
