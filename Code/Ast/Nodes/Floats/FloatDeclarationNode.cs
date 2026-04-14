using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Floats;

public class FloatDeclarationNode : SymbolNode
{
	internal FloatExpressionNode FloatExpression;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.FloatKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => this.Id = value);
		FloatExpression = ParseChild(new FloatExpressionNode());
	}
}

