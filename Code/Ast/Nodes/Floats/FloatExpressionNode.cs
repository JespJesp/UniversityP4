using System.Globalization;
using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Floats;

public class FloatExpressionNode : BranchNode
{
	private enum Operation
	{
		None, // Works for both addition and subtraction
		Multiplication,
		Division
	}
	private class Term
	{
		public float Value;
		public string StringValue = "";
		public bool IsIdentifier;
		public Operation Operation;
	};
	private List<Term> _terms = new();
	public float Value = 0;

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
				if (!_symbolTable.Contains<FloatDeclarationNode>(term.StringValue))
				{
					Validator.AddError(this, $"Float variable with ID '{term.StringValue}' is not declared.");
					return;
				}

				term.Value = _symbolTable.Get<FloatDeclarationNode>(term.StringValue).FloatExpression.Value;
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
						Annotator.AddError(this, "Illegal operation: Cannot divide with 0");
						return;
					}
					this.Value /= divisor;
					break;
			}
		}
	}
}