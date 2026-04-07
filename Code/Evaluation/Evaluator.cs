using Runtime;

namespace Evaluation;

public class Evaluator
{
	public void Evaluate()
	{
		// TODO: Remove; it's for debugging
		PrintToConsole();

		// TODO: Remove; it's for debugging, because the Timeline hasn't been implemented yet
		// RuntimeEnvironment.TheTimeline.Loops.Add(new(RuntimeEnvironment.Melodies["16_mainMelody"], 0, 10));
		// RuntimeEnvironment.TheTimeline.Loops.Add(new(RuntimeEnvironment.Melodies["32_longNotes"], 0, 18));
		// RuntimeEnvironment.TheTimeline.Loops.Add(new(RuntimeEnvironment.Melodies["16_loopingTest1of3"], 0, 64));
		// RuntimeEnvironment.TheTimeline.Loops.Add(new(RuntimeEnvironment.Melodies["16_loopingTest2of3"], 16, 64));
		// RuntimeEnvironment.TheTimeline.Loops.Add(new(RuntimeEnvironment.Melodies["16_loopingTest3of3"], 32, 64));

		// TODO: Remove; it's for debugging, because the Timeline hasn't been implemented yet
		Runtime.Environment.TheTimeline.Loops.Add(new(Runtime.Environment.Melodies["8_guitar"], 0, 8));
		Runtime.Environment.TheTimeline.Loops.Add(new(Runtime.Environment.Melodies["16_flute"], 12, 64));

		AudioRenderer.Render();
	}

	private void PrintToConsole() // TODO: Remove after debugging. It's just an example.
	{
		foreach (Pattern pattern in Runtime.Environment.Patterns.Values)
		{
			Console.WriteLine($"\n=== Pattern: {pattern.Id} ===");
			Console.WriteLine($"Length: {pattern.LengthInBeats}");

			Console.WriteLine("Children:");
			foreach (string patternAndMelodyIds in pattern.PatternAndMelodyIds)
			{
				Console.WriteLine($"  - {patternAndMelodyIds}");
			}
		}

		foreach (Melody melody in Runtime.Environment.Melodies.Values)
		{
			Console.WriteLine($"\n=== Melody: {melody.Id} ===");
			Console.WriteLine($"Length: {melody.LengthInBeats}");

			Console.WriteLine("Samples:");
			foreach (string sampleId in melody.SampleIds)
			{
				Sample sample = Runtime.Environment.Samples[sampleId];
				Console.WriteLine($"  - {sample.Id} = '{sample.FilePath}', reference note octave = {sample.ReferencePitch.Octave}");
			}

			Console.WriteLine("Notes:");
			foreach (Note note in melody.Notes)
			{
				Console.WriteLine($"  - Time: {note.StartBeat}-{note.EndBeat}, Octave: {note.ThePitch.Octave}, Pitch class: {note.ThePitch.PitchClass}");
			}
		}
	}
}