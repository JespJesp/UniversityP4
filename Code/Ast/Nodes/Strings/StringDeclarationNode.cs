using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Strings;

public class StringDeclarationNode : SymbolNode
{
	internal StringExpressionNode StringExpression;

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

