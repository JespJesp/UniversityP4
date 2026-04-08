using Ast.Nodes;
using Ast.Tables;
using Runtime;
using Runtime.Objects;

namespace Ast;

public static class Evaluator
{
	public static void EvaluateTree(ProgramNode programNode)
	{
		try
		{
			VariableTable globalVariables = new();
			programNode.CascadeEvaluate(new(), globalVariables);
			AudioRenderer.Render(globalVariables);

			PrintToConsole(globalVariables);
		}
		catch (Exception exception)
		{
			throw new Exception($"Evaluation error: {exception}");
		}
	}

	// TODO: Remove after debugging. It's just an example.
	private static void PrintToConsole(VariableTable globalVariables)
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
				Console.WriteLine($"  - '{sample.FilePath}', reference pitch = {sample.ReferencePitch.PitchClass}{sample.ReferencePitch.Octave}");
			}

			Console.WriteLine("Notes:");
			foreach (Note note in melody.Notes)
			{
				Console.WriteLine($"  - Time: {note.StartBeat}-{note.EndBeat}, Pitch: {note.Pitch0.PitchClass}{note.Pitch0.Octave}, Volume: {note.Volume}, Pan: {note.Pan}");
			}
		}
	}
}