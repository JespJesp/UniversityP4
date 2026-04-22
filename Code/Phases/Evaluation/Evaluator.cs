using Ast;
using Ast.Nodes;
using Runtime.AudioRendering;

namespace Phases.Evaluation;

public class Evaluator
{
	public void Evaluate(ProgramNode programNode, string inputFileFolderPath)
	{
		try
		{
			CascadeEvaluate(programNode);
			new AudioRenderer().RenderToFile(programNode.timelineNode, inputFileFolderPath);
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

