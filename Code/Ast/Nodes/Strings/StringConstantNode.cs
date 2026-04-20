using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Strings;

public class StringConstantNode : SymbolNode
{
	internal StringExpressionNode StringExpression = new();

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.Identifier, out this.Id);
		StringExpression = parser.ParseChild(this, new StringExpressionNode());
	}
}

