using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Strings;

public class StringDeclarationNode : SymbolNode
{
	internal StringExpressionNode StringExpression;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.StringKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => this.Id = value);
		StringExpression = ParseChild(new StringExpressionNode());
	}
}

