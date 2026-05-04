using System.Globalization;
using Phases.Annotation;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Floats;

public class FloatExpressionNode : Node
{
	public float Value = 0;
	public bool HasValue;
	private bool _isOptional;
	private List<Term> _terms = new();

	public FloatExpressionNode(bool isOptional = false)
	{
		_isOptional = isOptional;
	}

	private class Term
	{
		public float Value;
		public string RawValue = "";
		public bool IsIdentifier;
		public Operation Operation;
	};

	private enum Operation
	{
		Addition, // Works for both addition and subtraction
		Subtraction,
		Multiplication,
		Division
	}

	public override void CascadeParse(Parser parser)
	{
		Term? newTerm = new() { Operation = Operation.Addition };
		do
		{
			// Add term
			newTerm.IsIdentifier = parser.TryConsumeToken(TokenType.Identifier, out newTerm.RawValue);
			if (!newTerm.IsIdentifier)
			{
				if (_isOptional)
				{
					parser.TryConsumeToken(TokenType.Float, out newTerm.RawValue);
				}
				else
				{
					parser.ConsumeToken(TokenType.Float, out newTerm.RawValue);
				}
			}

			// Quit if the optional float expression is not used
			if (newTerm.RawValue == "")
			{
				HasValue = false;
				break;
			}
			else
			{
				HasValue = true;
			}

			_terms.Add(newTerm);

			// Check for more terms
			// NOTE: For example, "2 - 1" and "2-1" are both allowed expressions, but it's important to note that they must be handled differently, since "2 - 1" consists of 3 tokens (a float "2", a minus "-", and float "1"), while "2-1" consists of 2 tokens (a float "2", and a float "-1").
			newTerm = null;
			if (parser.TryConsumeToken(TokenType.Plus)
					|| TokenTypeExtensions.IsSubtypeOf(parser.CursorToken.Type, TokenType.Float) && parser.CursorToken.Value[0] == '-')
			{
				newTerm = new() { Operation = Operation.Addition };
			}
			else if (parser.TryConsumeToken(TokenType.Minus))
			{
				newTerm = new() { Operation = Operation.Subtraction };
			}
			else if (parser.TryConsumeToken(TokenType.Asterisk))
			{
				newTerm = new() { Operation = Operation.Multiplication };
			}
			else if (parser.TryConsumeToken(TokenType.Slash))
			{
				newTerm = new() { Operation = Operation.Division };
			}
		} while (newTerm is not null);
	}

	public override void Annotate(Annotator annotator)
	{
		foreach (Term term in _terms)
		{
			// Get term value
			if (term.IsIdentifier)
			{
				if (!SymbolTable.Contains<FloatConstantNode>(term.RawValue))
				{
					throw new Exception($"Float variable with ID '{term.RawValue}' is not declared");
				}

				term.Value = SymbolTable.Get<FloatConstantNode>(term.RawValue).FloatExpression.Value;
			}
			else
			{
				term.Value = float.Parse(term.RawValue, CultureInfo.InvariantCulture);
			}

			// Apply term to final result
			switch (term.Operation)
			{
				case Operation.Addition:
					Value += term.Value;
					break;
				case Operation.Subtraction:
					Value -= term.Value;
					break;
				case Operation.Multiplication:
					Value *= term.Value;
					break;
				case Operation.Division:
					float divisor = term.Value;
					if (divisor == 0)
					{
						throw new Exception("Illegal operation: Cannot divide with 0");
					}
					Value /= divisor;
					break;
			}
		}
	}
}