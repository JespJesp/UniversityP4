using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Strings;

public class StringExpressionNode : BranchNode
{
	internal List<StringValueNode> StringValueNodes = new();
	public Func<string> GetValue = () => throw new NotImplementedException("Internal error!");

	public StringExpressionNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		do
		{
			new StringValueNode(this);
		} while (Parser.TryConsumeToken(TokenType.Plus));
	}

	protected override void Annotate()
	{
		GetValue = () =>
		{
			string finalValue = "";
			foreach (StringValueNode stringValueNode in StringValueNodes)
			{
				finalValue += stringValueNode.GetValue();
			}
			return finalValue;
		};
	}
}