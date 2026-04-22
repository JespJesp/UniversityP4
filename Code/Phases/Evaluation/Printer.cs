using Runtime.Objects;
using Runtime.AudioRendering.Loops;

namespace Phases.Evaluation;

// TODO: Remove after debugging. It's just an example.
public class Printer
{
	public static void ExamplePrintToConsole(List<Loop> loops)
	{
		foreach (Loop loop in loops)
		{
			Melody melody = loop.Melody;

			Console.WriteLine($"\n=== Loop ====");
			Console.WriteLine($"Loop length: {loop.LengthInBeats}");
			Console.WriteLine($"Melody length: {melody.LengthInBeats}");

			Console.WriteLine("Samples:");
			foreach (Sample sample in melody.Samples)
			{
				Console.WriteLine($"  - '{sample.FilePath}', reference pitch octave = {sample.ReferencePitch.Octave}");
			}

			Console.WriteLine("Notes:");
			foreach (Note note in melody.Notes)
			{
				string sampleOverrideText = "";
				if (note.SampleOverride != null)
				{
					sampleOverrideText = $", Sample override: '{note.SampleOverride.FilePath}'";
				}

				Console.WriteLine($"  - Time: {note.StartBeat}-{note.EndBeat}, Pitch octave: {note.Pitch.Octave}, Volume: {note.Volume}, Pan: {note.Pan}{sampleOverrideText}");
			}
		}
	}
}

