using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using AbstractSyntax;

namespace Evaluation;

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

		foreach (string melodyId in AST.TheTimeline.MelodyIds)
		{
			Melody melody = AST.Melodies[melodyId];
			foreach (string sampleId in melody.SampleIds)
			{
				Sample sample = AST.Samples[sampleId];
				foreach (Note note in melody.Notes)
				{
					ISampleProvider sound = CreateSound(melody, sample, note);
					sounds.Add(sound);
				}
			}
		}

		return sounds;
	}

	private static ISampleProvider CreateSound(Melody melody, Sample sample, Note note)
	{
		// TODO: Don't use "Directory.GetCurrentDirectory()"
		var reader = new AudioFileReader(Directory.GetCurrentDirectory() + sample.FilePath);

		// Resample the sound to ensure it uses the song's sample rate
		var resampler = new WdlResamplingSampleProvider(reader, AST.SampleRate);

		var volumeProvider = new VolumeSampleProvider(resampler)
		{
			Volume = 1 // TODO: Implement volume control
		};

		var pitchShifter = new SmbPitchShiftingSampleProvider(volumeProvider)
		{
			PitchFactor = GetPitchFactor(sample.ReferencePitch, note.Pitch)
		};

		var offsetter = new OffsetSampleProvider(pitchShifter)
		{
			DelayBy = TimeSpan.FromSeconds(GetStartTime(note)),
			Take = TimeSpan.FromSeconds(GetDuration(note))
		};

		return offsetter;
	}

	private static float GetStartTime(Note note)
	{
		// TODO: Not implemented
		return 0;
	}

	private static float GetDuration(Note note)
	{
		// TODO: Not implemented
		return 0;
	}

	private static float GetPitchFactor(Pitch samplePitch, Pitch notePitch)
	{
		float pitchFactor = 1;

		// Going an octave up is the same as adding the pitch factor to the itself, so doubling it (for example, from 1.0 to 2.0)
		int octaveDifference = notePitch.Octave - samplePitch.Octave;
		pitchFactor *= MathF.Pow(2, octaveDifference);

		// Going a pitch class up is the same as adding 1/12 of the pitch factor to the itself
		int pitchClassDifference = notePitch.PitchClass - samplePitch.PitchClass;
		pitchFactor *= 1 + pitchClassDifference / 12f;

		return pitchFactor;
	}
}