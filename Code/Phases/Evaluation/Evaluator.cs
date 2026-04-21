using Ast;
using Ast.Nodes;
using Phases.Evaluation.AudioRendering;
using Runtime.Objects.Timelines;

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

			Printer.ExamplePrintToConsole(timeline);
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
}

