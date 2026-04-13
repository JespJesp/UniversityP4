using Ast.NodeArchetypes;
using Ast.Tables;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Primitives;

public class StringDeclarationNode : VariableNode
{
	public StringVariable StringVariable = new();
	protected override RuntimeObject GetRuntimeObject() => StringVariable;

	public StringDeclarationNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.StringKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => this.Id = value);
		new StringExpressionNode(this);
	}
}

