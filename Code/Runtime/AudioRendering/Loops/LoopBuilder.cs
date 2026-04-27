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

		foreach (TimelineCommand command in timeline.Commands)
		{
			switch (command.Type)
			{
				case TimelineCommandType.Start:
					ExecuteStartCommand(command, activeMelodies, commandTargets, globalSymbols);
					break;
				case TimelineCommandType.Stop:
					ExecuteStopCommand(command, Loops, activeMelodies, commandTargets, globalSymbols);
					break;
				default:
					throw new Exception($"Unexpected timeline command type: {command.Type}");
			}
		}

		CloseOpenLoops(timeline, activeMelodies);

		return Loops;
	}

	private void ExecuteStartCommand(
			TimelineCommand command,
			Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
			Dictionary<string, List<Melody>> commandTargets,
			SymbolTable globalSymbols)
	{
		float startBeat = command.Beat ?? 0;
		HashSet<Melody> melodies = TimelineCommandTargetResolver.ExpandTargetsToMelodies(command.TargetIds, globalSymbols);

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
			List<Loop> loops,
			Dictionary<Melody, List<ActiveLoopState>> activeMelodies,
			Dictionary<string, List<Melody>> commandTargets,
			SymbolTable globalSymbols)
	{
		if (!command.Beat.HasValue)
		{
			throw new Exception("Timeline stop command is missing a beat value");
		}

		float stopBeat = command.Beat.Value;
		HashSet<Melody> targetsToStop = ResolveStopTargets(command, activeMelodies, commandTargets, globalSymbols);

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

				Melody adjustedMelody = MelodyModifier.CreateAdjustedMelody(melody, start.GainMultiplier, start.PitchShiftHalfsteps);
				loops.Add(new Loop
				{
					Melody = adjustedMelody,
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

	private void CloseOpenLoops(Timeline timeline, Dictionary<Melody, List<ActiveLoopState>> activeMelodies)
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
					Melody adjustedMelody = MelodyModifier.CreateAdjustedMelody(melody, start.GainMultiplier, start.PitchShiftHalfsteps);
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

		public ActiveLoopState(float startBeat, float gainMultiplier, float pitchShiftHalfsteps)
		{
			StartBeat = startBeat;
			GainMultiplier = gainMultiplier;
			PitchShiftHalfsteps = pitchShiftHalfsteps;
		}
	}
}
