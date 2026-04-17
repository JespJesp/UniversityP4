using System.Globalization;
using Ast.NodeArchetypes;
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

	public override void CascadeParse()
	{
		Operation? newTermOperation = Operation.None;
		while (newTermOperation is not null)
		{
			Term newTerm = new()
			{
				Operation = newTermOperation.Value
			};

			newTerm.IsIdentifier = Parser.TryConsumeToken(TokenType.Identifier, (value) => newTerm.RawValue = value);
			if (!newTerm.IsIdentifier)
			{
				Parser.ConsumeToken(TokenType.Float, (value) => newTerm.RawValue = value);
			}

			_terms.Add(newTerm);

			newTermOperation = null;
			if (Parser.TryConsumeToken(TokenType.Plus)
				|| TokenTypeExtensions.IsSubtypeOf(Parser.CursorToken.Type, TokenType.Float) && Parser.CursorToken.Value[0] == '-')
			{
				newTermOperation = Operation.None;
			}
			else if (Parser.TryConsumeToken(TokenType.Asterisk))
			{
				newTermOperation = Operation.Multiplication;
			}
			else if (Parser.TryConsumeToken(TokenType.Slash))
			{
				newTermOperation = Operation.Division;
			}
		}
	}

	public override void Annotate()
	{
		foreach (Term term in _terms)
		{
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