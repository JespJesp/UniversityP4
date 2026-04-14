using System.Globalization;
using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Floats;

internal class FloatValueNode : BranchNode
{
	private string _value = "";
	private bool _isIdentifier = false;
	public Func<float> GetValue = () => throw new NotImplementedException("Internal error!");

	protected override void Parse()
	{
		_isIdentifier = Parser.TryConsumeToken(TokenType.Identifier, (value) => this._value = value);

		if (!_isIdentifier)
		{
			Parser.ConsumeToken(TokenType.Float, (value) => this._value = value);
		}

		// Parser.TryConsumeToken(TokenType.Identifier, (value) => this._value = value);
	}

	protected override void Annotate()
	{
		if (_isIdentifier)
		{
			GetValue = () => _symbolTable.Get<FloatDeclarationNode>(this._value).FloatExpression.GetValue();
		}
		else
		{
			GetValue = () => float.Parse(_value, CultureInfo.InvariantCulture);
		}
	}

	protected override void Validate()
	{
		try
		{
			if (_isIdentifier && !_symbolTable.Contains<FloatDeclarationNode>(this._value))
			{
				throw new Exception($"Float variable with ID '{this._value}' is not declared.");
			}
		}
		catch (Exception exception)
		{
			Validator.AddError(this, exception.Message);
			GetValue = () => 0;
		}
	}
}