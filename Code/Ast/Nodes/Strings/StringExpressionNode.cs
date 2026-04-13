using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Strings;

public class StringExpressionNode : BranchNode
{
	internal List<StringValueNode> StringValueNodes = new();

	public StringExpressionNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		do
		{
			StringValueNodes.Add(new StringValueNode(this));
		} while (Parser.TryConsumeToken(TokenType.Plus));
	}

	public string GetValue()
	{
		string finalValue = "";
		foreach (StringValueNode stringValueNode in StringValueNodes)
		{
			finalValue += stringValueNode.GetValue();
		}
		return finalValue;
	}
}