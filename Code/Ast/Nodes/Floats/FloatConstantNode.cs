using Ast.NodeArchetypes;
using Parsing;
using Lexing.Tokens;

namespace Ast.Nodes.Floats;

public class FloatConstantNode : SymbolNode
{
	internal FloatExpressionNode FloatExpression = new();

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.FloatKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => this.Id = value);
		FloatExpression = ParseChild(new FloatExpressionNode());
	}
}

