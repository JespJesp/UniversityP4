using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Strings;

public class StringExpressionNode : BranchNode
{
	internal List<StringValueNode> StringValueNodes = new();

	protected override void Parse()
	{
		do
		{
			StringValueNodes.Add(ParseChild(new StringValueNode()));
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