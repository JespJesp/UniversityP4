using Ast;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Phases.Annotation;
using Runtime;
using Runtime.Objects;
using Runtime.Objects.Timelines;

namespace Phases.Evaluation;

public class AudioRenderer
{
	const string OutputFileName = "ProgramOutput.wav";

	public void RenderToFile(Timeline timeline, string inputFileFolderPath)
	{
		List<ISampleProvider> sounds = CreateSounds(timeline, inputFileFolderPath);
		var mixer = new MixingSampleProvider(sounds);
		WaveFileWriter.CreateWaveFile16(OutputFileName, mixer);

		Console.WriteLine($"Successfully created audio file: '{OutputFileName}'");
	}

	private List<ISampleProvider> CreateSounds(Timeline timeline, string inputFileFolderPath)
	{
		List<ISampleProvider> sounds = new();

		foreach (Loop loop in timeline.Loops)
		{
			Melody melody = loop.Melody;

			foreach (Note note in melody.Notes)
			{
				List<Sample> samplesToRender;
				if (note.SampleOverride != null)
				{
					samplesToRender = new List<Sample> { note.SampleOverride };
				}
				else
				{
					samplesToRender = melody.Samples;
				}

				foreach (Sample sample in samplesToRender)
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

	private ISampleProvider CreateSound(Timeline timeline, Sample sample, Note note, float globalStartBeat, float durationInBeats, string inputFileFolderPath)
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
			PitchFactor = GetPitchFactor(sample.ReferencePitch, note.Pitch)
		};

		var envelopeProvider = new AdsrEnvelopeSampleProvider(
				pitchShifter,
				noteDurationSeconds: ConvertBeatsToSeconds(timeline, durationInBeats),
				attackSeconds: ConvertBeatsToSeconds(timeline, sample.AttackBeats),
				holdSeconds: ConvertBeatsToSeconds(timeline, sample.HoldBeats),
				decaySeconds: ConvertBeatsToSeconds(timeline, sample.DecayBeats),
				sustainLevel: sample.SustainLevel,
				releaseSeconds: ConvertBeatsToSeconds(timeline, sample.ReleaseBeats));

		var offsetter = new OffsetSampleProvider(envelopeProvider)
		{
			DelayBy = TimeSpan.FromSeconds(ConvertBeatsToSeconds(timeline, globalStartBeat + sample.DelayBeats)),
			Take = TimeSpan.FromSeconds(ConvertBeatsToSeconds(timeline, durationInBeats + sample.ReleaseBeats)) // Duration of sample
		};

		return offsetter;
	}

	private float ConvertBeatsToSeconds(Timeline timeline, float beats)
	{
		// TODO: Jesp: Does this do the right calculation?
		return beats * 60f / timeline.BeatsPerMinute * 4f / timeline.BeatNoteValue;
	}

	private float GetPitchFactor(Pitch samplePitch, Pitch notePitch)
	{
		int halfstepDifference = (notePitch.Octave - samplePitch.Octave) * 12
									  + (notePitch.PitchClass - samplePitch.PitchClass);
		return MathF.Pow(2, halfstepDifference / 12f);
	}
}