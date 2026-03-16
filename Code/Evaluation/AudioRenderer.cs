using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using AST;

namespace Evaluation;

public static class AudioRenderer
{
	const string OutputFileName = "ProgramOutput.wav";

	public static void Render(Song song)
	{
		List<ISampleProvider> sounds = new();

		AddSounds(sounds, song);

		var mixer = new MixingSampleProvider(sounds);
		WaveFileWriter.CreateWaveFile16(OutputFileName, mixer);
	}

	public static void AddSounds(List<ISampleProvider> sources, Song song)
	{
		foreach (string patternId in song.TheTimeline.PatternIds)
		{
			Pattern pattern = song.GetPattern(patternId);

			foreach (string sampleId in pattern.SampleIds)
			{
				Sample sample = song.GetSample(sampleId);

				foreach (Note note in pattern.Notes)
				{
					// TODO: Don't use "Directory.GetCurrentDirectory()"
					var reader = new AudioFileReader(Directory.GetCurrentDirectory() + sample.FilePath);
					var resampler = new WdlResamplingSampleProvider(reader, song.SampleRate); // Resample to esnure it uses the song's sample rate
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
					sources.Add(offsetter);
				}
			}
		}
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

		int octaveDifference = notePitch.Octave - samplePitch.Octave;
		pitchFactor *= MathF.Pow(2, octaveDifference);

		int pitchClassDifference = notePitch.PitchClass - samplePitch.PitchClass;
		pitchFactor *= 1 + pitchClassDifference / 12f;

		return pitchFactor;
	}
}