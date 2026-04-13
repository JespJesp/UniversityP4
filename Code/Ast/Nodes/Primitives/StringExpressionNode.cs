using Ast.NodeArchetypes;
using Ast.Tables;
using Lexing.Tokens;
using Runtime.Objects;

namespace Ast.Nodes.Primitives;

public class StringExpressionNode : BranchNode
{
	private string _value = "";
	private bool _isIdentifier = false;
	public Func<string> GetValue;

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

	protected override void Validate(SemanticSymbolTable symbols)
	{
		// I need to assign the GetValue func here

		if (_isIdentifier && !symbols.Contains(typeof(StringVariable), this._value))
		{
			Validator.AddError(this, $"String variable with ID '{this._value}' is not declared.");
		}
	}

	protected override void Evaluate(RuntimeVariableTable localVariables)
	{
		if (_isIdentifier)
		{
			GetValue = () => localVariables.Get<StringVariable>(this._value).Value;
		}
		else
		{
			GetValue = () => _value;
		}
	}
}