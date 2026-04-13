using Ast.NodeArchetypes;
using Lexing.Tokens;
using Runtime.Objects;

namespace Ast.Nodes.Primitives;

public class StringExpressionNode : BranchNode
{
	private string _value = "";
	private bool _isIdentifier = false;
	public Func<string> GetValue = () => throw new NotImplementedException("Internal error!");

	public StringExpressionNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		// TODO: Add concatenation via "+" symbol.

		_isIdentifier = Parser.TryConsumeToken(TokenType.Identifier, (value) => this._value = value);

		if (!_isIdentifier)
		{
			Parser.ConsumeToken(TokenType.String, (value) => this._value = value);
		}
	}

	protected override void Annotate()
	{
		// I need to assign the GetValue func here

		if (_isIdentifier && !_symbolTable.Contains<StringVariable>(this._value))
		{
			Annotator.AddError(this, $"String variable with ID '{this._value}' is not declared.");
		}
	}

	protected override void Evaluate()
	{
		if (_isIdentifier)
		{
			GetValue = () => _symbolTable.Get<StringVariable>(this._value).Value;
		}
		else
		{
			GetValue = () => _value;
		}
	}
}