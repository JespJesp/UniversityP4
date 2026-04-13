using Ast.NodeArchetypes;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Primitives;

public class StringDeclarationNode : SymbolNode
{
	public StringExpressionNode StringExpression;

	public StringDeclarationNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.StringKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => this.Id = value);
		StringExpression = new StringExpressionNode(this);
	}
}

