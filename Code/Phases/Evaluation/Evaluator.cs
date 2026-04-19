using Ast;
using Ast.Nodes;
using Runtime.Objects;
using Runtime.Objects.Timeline;

namespace Phases.Evaluation;

public class Evaluator
{
	public void Evaluate(ProgramNode programNode, string inputFileFolderPath)
	{
		try
		{
			CascadeEvaluate(programNode);

			Timeline timeline = programNode.timelineNode.Timeline;
			timeline.BuildLoopsFromCommands(programNode.timelineNode.SymbolTable);

			new AudioRenderer().RenderToFile(timeline, inputFileFolderPath);

			ExamplePrintToConsole(timeline);
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
	private void ExamplePrintToConsole(Timeline timeline)
	{
		foreach (Loop loop in timeline.Loops)
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

