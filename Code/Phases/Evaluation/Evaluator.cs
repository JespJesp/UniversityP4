using Ast;
using Ast.Nodes;
using Ast.Nodes.Timelines;
using Runtime.AudioRendering;

namespace Phases.Evaluation;

public class Evaluator
{
	private List<string> _errors = new();

	public void Evaluate(FileNode rootNode, FileInfo outputFile)
	{
		_errors.Clear();

		try
		{
			CascadeEvaluate(rootNode);

			if (_errors.Any())
			{
				throw new Exception("\n- " + string.Join("\n- ", _errors));
			}

			new AudioRenderer().RenderToFile(TimelineNode.Instance, outputFile);
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
			_errors.Add($"{node.Location}. Node: '{node.GetType()}'. {exception.Message}");
		}

		foreach (Node child in node.Children)
		{
			CascadeEvaluate(child);
		}
	}
}

