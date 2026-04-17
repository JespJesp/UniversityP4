using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Strings;

public class StringConstantNode : SymbolNode
{
	internal StringExpressionNode StringExpression = new();

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.StringKeyword);
		parser.ConsumeToken(TokenType.Identifier, (value) => this.Id = value);
		StringExpression = parser.ParseChild(this, new StringExpressionNode());
	}
}

