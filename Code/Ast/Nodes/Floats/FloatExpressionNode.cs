using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Floats;

public class FloatExpressionNode : BranchNode
{
	internal List<FloatValueNode> FloatValueNodes = new();

	public FloatExpressionNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		// TODO: Implement minus, multiplication, and division

		do
		{
			FloatValueNodes.Add(new FloatValueNode(this));
		} while (Parser.TryConsumeToken(TokenType.Plus));
	}

	public float GetValue()
	{
		float finalValue = 0;
		foreach (FloatValueNode floatValueNode in FloatValueNodes)
		{
			finalValue += floatValueNode.GetValue();
		}
		return finalValue;
	}
}