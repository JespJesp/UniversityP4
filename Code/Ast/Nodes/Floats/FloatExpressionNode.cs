using System.Globalization;
using Phases.Annotation;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Floats;

public class FloatExpressionNode : Node
{
	public float Value = 0;

	private List<Term> _terms = new();

	private class Term
	{
		public float Value;
		public string RawValue = "";
		public bool IsIdentifier;
		public Operation Operation;
	};

	private enum Operation
	{
		None, // Works for both addition and subtraction
		Multiplication,
		Division
	}

	public override void CascadeParse(Parser parser)
	{
		Term? newTerm = new() { Operation = Operation.None };
		do
		{
			// Add term
			newTerm.IsIdentifier = parser.TryConsumeToken(TokenType.Identifier, (value) => newTerm.RawValue = value);
			if (!newTerm.IsIdentifier)
			{
				parser.ConsumeToken(TokenType.Float, (value) => newTerm.RawValue = value);
			}
			_terms.Add(newTerm);

			// Check for more terms
			newTerm = null;
			if (parser.TryConsumeToken(TokenType.Plus)
					|| TokenTypeExtensions.IsSubtypeOf(parser.CursorToken.Type, TokenType.Float) && parser.CursorToken.Value[0] == '-')
			{
				newTerm = new() { Operation = Operation.None };
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
					throw new Exception($"Float variable with ID '{term.RawValue}' is not declared.");
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
				case Operation.None:
					this.Value += term.Value;
					break;
				case Operation.Multiplication:
					this.Value *= term.Value;
					break;
				case Operation.Division:
					float divisor = term.Value;
					if (divisor == 0)
					{
						throw new Exception("Illegal operation: Cannot divide with 0");
					}
					this.Value /= divisor;
					break;
			}
		}
	}
}