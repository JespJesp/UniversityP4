using AbstractSyntax;

namespace Evaluation;

public class Evaluator
{
	public void Evaluate()
	{
		// TODO: Remove; it's for debugging
		PrintToConsole();

		// TODO: Remove; it's for debugging
		AST.TheTimeline.Loops.Add(new(AST.Melodies["16_mainMelody"], 0, 48));
		AST.TheTimeline.Loops.Add(new(AST.Melodies["16_loopingTest1of3"], 0, 64));
		AST.TheTimeline.Loops.Add(new(AST.Melodies["16_loopingTest2of3"], 16, 64));
		AST.TheTimeline.Loops.Add(new(AST.Melodies["16_loopingTest3of3"], 32, 64));

		AudioRenderer.Render();
	}

	private void PrintToConsole() // TODO: Remove after debugging. It's just an example.
	{
		foreach (Pattern pattern in AST.Patterns.Values)
		{
			Console.WriteLine($"\n=== Pattern: {pattern.Id} ===");
			Console.WriteLine($"Length: {pattern.Length}");

			Console.WriteLine("Children:");
			foreach (string patternAndMelodyIds in pattern.PatternAndMelodyIds)
			{
				Console.WriteLine($"  - {patternAndMelodyIds}");
			}
		}

		foreach (Melody melody in AST.Melodies.Values)
		{
			Console.WriteLine($"\n=== Melody: {melody.Id} ===");
			Console.WriteLine($"Length: {melody.Length}");

			Console.WriteLine("Samples:");
			foreach (string sampleId in melody.SampleIds)
			{
				Sample sample = AST.Samples[sampleId];
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