using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Floats;

public class FloatDeclarationNode : SymbolNode
{
	internal FloatExpressionNode FloatExpression;

	public FloatDeclarationNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.FloatKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => this.Id = value);
		FloatExpression = new FloatExpressionNode(this);
	}
}

