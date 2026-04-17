
using Ast;
using Ast.Nodes;
using Runtime;
using Runtime.Objects;

namespace Phases.Evaluation;

public class Evaluator
{
	public void Evaluate(ProgramNode programNode, string inputFileFolderPath)
	{
		try
		{
			CascadeEvaluate(programNode);
			new AudioRenderer().RenderToFile(inputFileFolderPath);
			ExamplePrintToConsole();
		}
		catch (Exception exception)
		{
			throw new Exception($"Evaluation error: {exception}");
		}
	}

	private void CascadeEvaluate(Node node)
	{
		try
		{
			node.Evaluate(this);
		}
		catch (Exception exception)
		{
			throw new Exception($"Line: {node.Line}. Column: {node.Column}. Node type: {node.GetType()}. {exception.Message}");
		}

		foreach (Node child in node.Children)
		{
			CascadeEvaluate(child);
		}
	}

	// TODO: Remove after debugging. It's just an example.
	private void ExamplePrintToConsole()
	{
		foreach (Loop loop in Timeline.Loops)
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
				Console.WriteLine($"  - Time: {note.StartBeat}-{note.EndBeat}, Pitch octave: {note.Pitch.Octave}, Volume: {note.Volume}, Pan: {note.Pan}");
			}
		}
	}
}