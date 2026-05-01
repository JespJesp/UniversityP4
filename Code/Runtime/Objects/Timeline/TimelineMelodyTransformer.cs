namespace Runtime.Objects;

internal static class TimelineMelodyTransformer
{
	public static Melody CreateAdjustedMelody(Melody source, float gainMultiplier, float pitchShiftHalfsteps, float panOffset)
	{
		if (gainMultiplier == 1.0f && pitchShiftHalfsteps == 0.0f && panOffset == 0.0f)
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
				Pitch0 = ShiftPitch(sourceNote.Pitch0, pitchShiftHalfsteps)
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
}
