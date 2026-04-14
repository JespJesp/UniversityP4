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
			Timeline timeline = globalVariables.Get<Timeline>("timeline");
			timeline.BuildLoopsFromCommands(globalVariables);
			AudioRenderer.RenderToFile(globalVariables, inputFileFolderPath);

			ExamplePrintToConsole(globalVariables);
		}
		catch (Exception exception)
		{
			throw new Exception($"Evaluation error: {exception}");
		}
	}

	// TODO: Remove after debugging. It's just an example.
	private static void ExamplePrintToConsole(RuntimeVariableTable globalVariables)
	{
		Timeline timeline = globalVariables.Get<Timeline>("timeline");

		foreach (Loop loop in timeline.Loops)
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
				Console.WriteLine($"  - Time: {note.StartBeat}-{note.EndBeat}, Pitch octave: {note.Pitch0.Octave}, Volume: {note.Volume}, Pan: {note.Pan}");
			}
		}
	}
}