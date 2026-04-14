using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Floats;

public class FloatExpressionNode : BranchNode
{
	internal List<FloatValueNode> FloatValueNodes = new();

	protected override void Parse()
	{
		// TODO: Implement minus, multiplication, and division

		do
		{
			FloatValueNodes.Add(ParseChild(new FloatValueNode()));
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