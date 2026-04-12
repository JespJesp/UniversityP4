using Ast.Nodes;
using Ast.Tables;
using Runtime;
using Runtime.Objects;

namespace Ast;

public static class Evaluator
{
	public static void EvaluateTree(ProgramNode programNode, string inputFileFolderPath)
	{
		try
		{
			RuntimeVariableTable globalVariables = new();
			programNode.CascadeEvaluate(new(), globalVariables);
			AudioRenderer.RenderToFile(globalVariables, inputFileFolderPath);

			ExamplePrintToConsole();
		}
		catch (Exception exception)
		{
			throw new Exception($"Evaluation error: {exception}");
		}
	}

	// TODO: Remove after debugging. It's just an example.
	private static void ExamplePrintToConsole()
	{
		foreach (Loop loop in Timeline.Loops)
		{
			Melody melody = loop.Melody0;

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
					sampleOverrideText = $", Sample: '{note.SampleOverride.FilePath}'";
				}

				Console.WriteLine($"  - Time: {note.StartBeat}-{note.EndBeat}, Pitch octave: {note.Pitch0.Octave}, Volume: {note.Volume}, Pan: {note.Pan}{sampleOverrideText}");
			}
		}
	}
}