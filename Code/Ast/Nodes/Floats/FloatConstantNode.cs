using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Floats;

public class FloatConstantNode : SymbolNode
{
	internal FloatExpressionNode FloatExpression = new();

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.Identifier, out Id);

		FloatExpression = parser.ParseChild(this, new FloatExpressionNode());
	}
}

