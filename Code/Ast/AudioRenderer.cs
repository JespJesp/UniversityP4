using Ast.Tables;
using Runtime.Objects;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Runtime;

namespace Ast;

public static class AudioRenderer
{
	const string OutputFileName = "ProgramOutput.wav";

	public static void RenderToFile(RuntimeVariableTable globalVariables, string inputFileFolderPath)
	{
		List<ISampleProvider> sounds = CreateSounds(globalVariables, inputFileFolderPath);
		var mixer = new MixingSampleProvider(sounds);
		WaveFileWriter.CreateWaveFile16(OutputFileName, mixer);
	}

	public static List<ISampleProvider> CreateSounds(RuntimeVariableTable globalVariables, string inputFileFolderPath)
	{
		List<ISampleProvider> sounds = new();
		Timeline timeline = globalVariables.Get<Timeline>("timeline");

		foreach (Loop loop in timeline.Loops)
		{
			Melody melody = loop.Melody0;
			foreach (Sample sample in melody.Samples)
			{
				foreach (Note note in melody.Notes)
				{
					float loops = loop.LengthInBeats / melody.LengthInBeats;

					int wholeLoops = (int)Math.Floor(loops);
					for (int i = 0; i < wholeLoops; i++)
					{
						float melodyStartBeat = loop.StartBeat + i * melody.LengthInBeats;

						float globalStartBeat = melodyStartBeat + note.StartBeat;

						float durationInBeats = note.EndBeat - note.StartBeat;

						ISampleProvider sound = CreateSound(timeline, sample, note, globalStartBeat, durationInBeats, inputFileFolderPath);
						sounds.Add(sound);
					}

					float loopsRemainder = loops - wholeLoops;
					if (loopsRemainder != 0)
					{
						float melodyStartBeat = loop.StartBeat + wholeLoops * melody.LengthInBeats;
						if (melodyStartBeat + note.StartBeat >= loop.LengthInBeats)
						{
							continue; // Skip "dead" notes that are played afte the loop has ended
						}

						float globalStartBeat = melodyStartBeat + note.StartBeat;

						float durationInBeatsMax = loop.LengthInBeats - wholeLoops * melody.LengthInBeats - note.StartBeat;
						float unclampedDurationInBeats = note.EndBeat - note.StartBeat;
						float durationInBeats = Math.Clamp(unclampedDurationInBeats, 0, durationInBeatsMax);

						ISampleProvider sound = CreateSound(timeline, sample, note, globalStartBeat, durationInBeats, inputFileFolderPath);
						sounds.Add(sound);
					}
				}
			}
		}

		return sounds;
	}

	private static ISampleProvider CreateSound(Timeline timeline, Sample sample, Note note, float globalStartBeat, float durationInBeats, string inputFileFolderPath)
	{
		var reader = new AudioFileReader(inputFileFolderPath + sample.FilePath);

		// Resample the sound to ensure it uses the output's sample rate
		var resampler = new WdlResamplingSampleProvider(reader, timeline.SampleRate);

		var volumeProvider = new VolumeSampleProvider(resampler)
		{
			Volume = note.Volume
		};

		// TODO: Implement panning

		var pitchShifter = new SmbPitchShiftingSampleProvider(volumeProvider)
		{
			PitchFactor = GetPitchFactor(sample.ReferencePitch, note.Pitch0)
		};

		var offsetter = new OffsetSampleProvider(pitchShifter)
		{
			DelayBy = TimeSpan.FromSeconds(ConvertBeatsToSeconds(timeline, globalStartBeat)),
			Take = TimeSpan.FromSeconds(ConvertBeatsToSeconds(timeline, durationInBeats)) // duration of sample
		};

		return offsetter;
	}

	private static float ConvertBeatsToSeconds(Timeline timeline, float beats)
	{
		return beats * 60f / timeline.BeatsPerMinute * 4f / timeline.BeatNoteValue;
	}

	private static float GetPitchFactor(Pitch samplePitch, Pitch notePitch)
	{
		int halfstepDifference = (notePitch.Octave - samplePitch.Octave) * 12
									  + (notePitch.PitchClass - samplePitch.PitchClass);
		return MathF.Pow(2, halfstepDifference / 12f);
	}
}