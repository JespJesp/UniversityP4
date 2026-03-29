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

		foreach (Loop loop in RuntimeEnvironment.TheTimeline.Loops)
		{
			Melody melody = loop.TheMelody;
			foreach (string sampleId in melody.SampleIds)
			{
				Sample sample = RuntimeEnvironment.Samples[sampleId];
				foreach (Note note in melody.Notes)
				{
					float loops = loop.LengthInBeats / loop.TheMelody.LengthInBeats;

					int wholeLoops = (int)Math.Floor(loops);
					for (int i = 0; i < wholeLoops; i++)
					{
						float melodyStartBeat = loop.StartBeat + i * loop.TheMelody.LengthInBeats;

						float globalStartBeat = melodyStartBeat + note.StartBeat;

						float durationInBeats = note.EndBeat - note.StartBeat;

						ISampleProvider sound = CreateSound(sample, note, globalStartBeat, durationInBeats);
						sounds.Add(sound);
					}

					float loopsRemainder = loops - wholeLoops;
					if (loopsRemainder != 0)
					{
						float melodyStartBeat = loop.StartBeat + wholeLoops * loop.TheMelody.LengthInBeats;
						if (melodyStartBeat + note.StartBeat >= loop.LengthInBeats)
						{
							continue; // Skip "dead" notes that are played afte the loop has ended
						}

						float globalStartBeat = melodyStartBeat + note.StartBeat;

						float durationInBeatsMax = loop.LengthInBeats - wholeLoops * loop.TheMelody.LengthInBeats - note.StartBeat;
						float unclampedDurationInBeats = note.EndBeat - note.StartBeat;
						float durationInBeats = Math.Clamp(unclampedDurationInBeats, 0, durationInBeatsMax);

						ISampleProvider sound = CreateSound(sample, note, globalStartBeat, durationInBeats);
						sounds.Add(sound);
					}
				}
			}
		}

		return sounds;
	}

	private static ISampleProvider CreateSound(Sample sample, Note note, float globalStartBeat, float durationInBeats)
	{
		// TODO: Don't use "Directory.GetCurrentDirectory()"
		var reader = new AudioFileReader(Directory.GetCurrentDirectory() + sample.FilePath);

		// Resample the sound to ensure it uses the AST's sample rate
		var resampler = new WdlResamplingSampleProvider(reader, RuntimeEnvironment.SampleRate);

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
			DelayBy = TimeSpan.FromSeconds(ConvertBeatsToSeconds(globalStartBeat)),
			Take = TimeSpan.FromSeconds(ConvertBeatsToSeconds(durationInBeats))
		};

		return offsetter;
	}

	private static float ConvertBeatsToSeconds(float beats)
	{
		return beats / RuntimeEnvironment.BeatNoteValue * 60f / RuntimeEnvironment.BeatsPerMinute;
	}

	private static float GetPitchFactor(Pitch samplePitch, Pitch notePitch)
	{
		int halfstepDifference = (notePitch.Octave - samplePitch.Octave) * 12
									  + (notePitch.PitchClass - samplePitch.PitchClass);
		return MathF.Pow(2, halfstepDifference / 12f);
	}
}