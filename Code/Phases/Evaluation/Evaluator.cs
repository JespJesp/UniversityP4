using Ast;
using Ast.Nodes;
using Ast.Nodes.Timelines;
using Runtime.AudioRendering;
using Runtime.AudioRendering.Loops;
using Runtime.Objects.Timelines;

namespace Phases.Evaluation;

public class Evaluator
{
	public void Evaluate(ProgramNode programNode, string inputFileFolderPath)
	{
		try
		{
			CascadeEvaluate(programNode);

			TimelineNode timelineNode = programNode.timelineNode;
			Timeline timeline = timelineNode.Timeline;
			SymbolTable globalSymbols = timelineNode.SymbolTable;

			var loops = new LoopBuilder().Build(timeline, globalSymbols);
			new AudioRenderer().RenderToFile(timeline, loops, inputFileFolderPath);

			Printer.ExamplePrintToConsole(loops);
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

