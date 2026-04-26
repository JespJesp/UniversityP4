using Ast;
using Ast.Nodes;
using Runtime.AudioRendering;

namespace Phases.Evaluation;

public class Evaluator
{
	private List<string> _errors = new();

	public void Evaluate(ProgramNode programNode, string inputFileFolderPath)
	{
		_errors.Clear();

		try
		{
			CascadeEvaluate(programNode);

			if (_errors.Any())
			{
				throw new Exception("\n- " + string.Join("\n- ", _errors));
			}

			new AudioRenderer().RenderToFile(programNode.timelineNode, inputFileFolderPath);
		}
		catch (Exception exception)
		{
			throw new Exception($"Evaluation errors: {exception}");
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
			_errors.Add($"{node.CursorInfo}. Node: '{node.GetType()}'. {exception.Message}");
		}

		foreach (Node child in node.Children)
		{
			CascadeEvaluate(child);
		}
	}
}

