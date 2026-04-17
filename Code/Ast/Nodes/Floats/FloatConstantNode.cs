using Ast.NodeArchetypes;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Floats;

public class FloatConstantNode : SymbolNode
{
	internal FloatExpressionNode FloatExpression = new();

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.FloatKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => this.Id = value);
		FloatExpression = Parser.ParseChild(this, new FloatExpressionNode());
	}
}

