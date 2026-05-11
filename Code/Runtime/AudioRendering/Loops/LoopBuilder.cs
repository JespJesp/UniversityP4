using Ast;
using Runtime.Objects;
using Runtime.Objects.Timelines;

namespace Runtime.AudioRendering.Loops;

public class LoopBuilder
{
	public List<Loop> Loops = new();

	public List<Loop> Build(Timeline timeline, SymbolTable globalSymbols)
	{
		Loops.Clear();

		var activeMelodies = new Dictionary<Melody, List<ActiveLoopState>>();
		var commandTargets = new Dictionary<string, List<Melody>>();

		// Track global beats
		float lastStartComputedBeat = 0f;
		float lastStopComputedBeat = 0f;
		float startChainOrigin = 0f;
		float stopChainOrigin = 0f;
		TimelineCommandType? prevCommandType = null;
		bool hasSeenStart = false;
		bool hasSeenStop = false;

		foreach (TimelineCommand command in timeline.Commands)
		{
			switch (command.Type)
			{
				case TimelineCommandType.Start:
				{
					// Start beats are relative to the last stop
					float baseline;
					if (hasSeenStop)
					{
						baseline = (prevCommandType == TimelineCommandType.Stop) ? stopChainOrigin : lastStopComputedBeat;
					}
					else
					{
						baseline = 0f;
					}

					float startBeat = baseline + (command.Beat ?? 0f);

					if (prevCommandType != TimelineCommandType.Start)
					{
						startChainOrigin = startBeat;
					}
					hasSeenStart = true;
					lastStartComputedBeat = startBeat;

					ExecuteStartCommand(command, activeMelodies, commandTargets, globalSymbols, startBeat);
					prevCommandType = TimelineCommandType.Start;
					break;
				}
				case TimelineCommandType.Stop:
				{
					if (!command.Beat.HasValue)
					{
						throw new Exception("Timeline stop command is missing a beat value");
					}

					// Stop beats are relative to the last start
					float baseline;
					if (hasSeenStart)
					{
						baseline = (prevCommandType == TimelineCommandType.Start) ? startChainOrigin : lastStartComputedBeat;
					}
					else
					{
						baseline = 0f;
					}

					float stopBeat = baseline + command.Beat.Value;
					
					if (prevCommandType != TimelineCommandType.Stop)
					{
						stopChainOrigin = stopBeat;
					}
					hasSeenStop = true;
					lastStopComputedBeat = stopBeat;

					ExecuteStopCommand(command, Loops, activeMelodies, commandTargets, globalSymbols, stopBeat);
					prevCommandType = TimelineCommandType.Stop;
					break;
				}
				default:
					throw new Exception($"Unexpected timeline command type: {command.Type}");
			}
		}

		// Determine final cursor for closing open loops
		float finalCursor = 0f;
		if (prevCommandType == TimelineCommandType.Start)
			finalCursor = lastStartComputedBeat;
		else if (prevCommandType == TimelineCommandType.Stop)
			finalCursor = lastStopComputedBeat;

		CloseOpenLoops(timeline, activeMelodies, finalCursor);

		return Loops;
	}

	private void ExecuteStartCommand(
			TimelineCommand command,
			Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
			Dictionary<string, List<Melody>> commandTargets,
			SymbolTable globalSymbols,
			float startBeat)
	{
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
	}

	private void ExecuteStopCommand(
			TimelineCommand command,
			List<Loop> loops,
			Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
			Dictionary<string, List<Melody>> commandTargets,
			SymbolTable globalSymbols,
			float stopBeat)
	{
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
					Melody = adjustedMelody,
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
	}

	private HashSet<Melody> ResolveStopTargets(
			TimelineCommand command,
			Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
			Dictionary<string, List<Melody>> commandTargets,
			SymbolTable globalSymbols)
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
			return TimelineCommandTargetResolver.ExpandTargetsToMelodies(command.TargetIds, globalSymbols);
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
						Melody = adjustedMelody,
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
