using Ast.NodeArchetypes;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Strings;

public class StringConstantNode : SymbolNode
{
	internal StringExpressionNode StringExpression = new();

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.StringKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => this.Id = value);
		StringExpression = Parser.ParseChild(this, new StringExpressionNode());
	}
}

