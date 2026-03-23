using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using AbstractSyntax;

namespace Evaluation;

// TODO: Implement support for time signature 

public static class AudioRenderer
{
	const string OutputFileName = "ProgramOutput.wav";

	public static void Render()
	{
		List<ISampleProvider> sounds = CreateSounds();
		var mixer = new MixingSampleProvider(sounds);
		WaveFileWriter.CreateWaveFile16(OutputFileName, mixer);
	}

	public static List<ISampleProvider> CreateSounds()
	{
		List<ISampleProvider> sounds = new();

		foreach (Loop loop in AST.TheTimeline.Loops)
		{
			Melody melody = loop.TheMelody;
			foreach (string sampleId in melody.SampleIds)
			{
				Sample sample = AST.Samples[sampleId];
				foreach (Note note in melody.Notes)
				{
					int loops = (int)Math.Floor(loop.Length / loop.TheMelody.Length);

					for (int i = 0; i < loops; i++)
					{
						float melodyStartBeat = loop.StartBeat + i * loop.TheMelody.Length;
						ISampleProvider sound = CreateSound(melodyStartBeat, sample, note);
						sounds.Add(sound);
					}

					// TODO: Add playing loops not the whole way through (e.g. a loop with a length of 2 beats, but its melody has the length 4)
				}
			}
		}

		return sounds;
	}

	private static ISampleProvider CreateSound(float melodyStartBeat, Sample sample, Note note)
	{
		// TODO: Don't use "Directory.GetCurrentDirectory()"
		var reader = new AudioFileReader(Directory.GetCurrentDirectory() + sample.FilePath);

		// Resample the sound to ensure it uses the AST's sample rate
		var resampler = new WdlResamplingSampleProvider(reader, AST.SampleRate);

		var volumeProvider = new VolumeSampleProvider(resampler)
		{
			Volume = 1 // TODO: Implement volume control
		};

		var pitchShifter = new SmbPitchShiftingSampleProvider(volumeProvider)
		{
			PitchFactor = GetPitchFactor(sample.ReferencePitch, note.ThePitch)
		};

		var offsetter = new OffsetSampleProvider(pitchShifter)
		{
			DelayBy = TimeSpan.FromSeconds(GetNoteStartTime(melodyStartBeat, note)),
			Take = TimeSpan.FromSeconds(GetNoteDuration(note))
		};

		return offsetter;
	}

	private static float GetNoteStartTime(float melodyStartBeat, Note note)
	{
		return ConvertBeatsToSeconds(melodyStartBeat) + ConvertBeatsToSeconds(note.StartBeat);
	}

	private static float GetNoteDuration(Note note)
	{
		return ConvertBeatsToSeconds(note.EndBeat) - ConvertBeatsToSeconds(note.StartBeat);
	}

	private static float ConvertBeatsToSeconds(float beats)
	{
		return beats / AST.BeatNoteValue * 60f / AST.BeatsPerMinute;
	}

	private static float GetPitchFactor(Pitch samplePitch, Pitch notePitch)
	{
		int halfstepDifference = (notePitch.Octave - samplePitch.Octave) * 12
									  + (notePitch.PitchClass - samplePitch.PitchClass);
		return MathF.Pow(2, halfstepDifference / 12f);
	}
}