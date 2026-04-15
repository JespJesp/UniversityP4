using System.Globalization;
using Parsing;
using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Floats;

public class FloatExpressionNode : BranchNode
{
	public float Value = 0;

	private List<Term> _terms = new();

	private class Term
	{
		public float Value;
		public string StringValue = "";
		public bool IsIdentifier;
		public Operation Operation;
	};

	private enum Operation
	{
		None, // Works for both addition and subtraction
		Multiplication,
		Division
	}

	protected override void Parse()
	{
		Operation? newTermOperation = Operation.None;
		while (newTermOperation is not null)
		{
			Term newTerm = new()
			{
				Operation = newTermOperation.Value
			};

			newTerm.IsIdentifier = Parser.TryConsumeToken(TokenType.Identifier, (value) => newTerm.StringValue = value);
			if (!newTerm.IsIdentifier)
			{
				Parser.ConsumeToken(TokenType.Float, (value) => newTerm.StringValue = value);
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

	protected override void Annotate()
	{
		foreach (Term term in _terms)
		{
			if (term.IsIdentifier)
			{
				if (!_symbolTable.Contains<FloatConstantNode>(term.StringValue))
				{
					throw new Exception($"Float variable with ID '{term.StringValue}' is not declared.");
				}

				term.Value = _symbolTable.Get<FloatConstantNode>(term.StringValue).FloatExpression.Value;
			}
			else
			{
				term.Value = float.Parse(term.StringValue, CultureInfo.InvariantCulture);
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