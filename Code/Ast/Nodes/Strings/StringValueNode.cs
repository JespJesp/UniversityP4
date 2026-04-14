using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Strings;

internal class StringValueNode : BranchNode
{
	private string _value = "";
	private bool _isIdentifier = false;
	public Func<string> GetValue = () => throw new NotImplementedException("Internal error!");

	protected override void Parse()
	{
		_isIdentifier = Parser.TryConsumeToken(TokenType.Identifier, (value) => this._value = value);

		if (!_isIdentifier)
		{
			Parser.ConsumeToken(TokenType.String, (value) => this._value = value);
		}
	}

	protected override void Annotate()
	{
		if (_isIdentifier)
		{
			GetValue = () => _symbolTable.Get<StringDeclarationNode>(this._value).StringExpression.GetValue();
		}
		else
		{
			GetValue = () => _value;
		}
	}

	protected override void Validate()
	{
		try
		{
			if (_isIdentifier && !_symbolTable.Contains<StringDeclarationNode>(this._value))
			{
				throw new Exception($"String variable with ID '{this._value}' is not declared.");
			}
		}
		catch (Exception exception)
		{
			Validator.AddError(this, exception.Message);
			GetValue = () => "";
		}
	}
}