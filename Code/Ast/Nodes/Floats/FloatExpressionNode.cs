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
		Addition,
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
					|| parser.CursorToken.Type == TokenType.Float && parser.CursorToken.Value[0] == '-')
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
		// Assign term values
		foreach (Term term in _terms)
		{
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
		}

		Value = CombineTerms(_terms);
	}

	private float CombineTerms(List<Term> terms)
	{
		if (terms.Count == 0)
		{
			return 0;
		}

		Stack<float> stack = new Stack<float>();
		stack.Push(terms[0].Value);

		for (int i = 1; i < terms.Count; i++)
		{
			var term = terms[i];

			switch (term.Operation)
			{
				case Operation.Multiplication:
					stack.Push(stack.Pop() * term.Value);
					break;
				case Operation.Division:
					stack.Push(stack.Pop() / term.Value);
					break;
				case Operation.Addition:
					stack.Push(term.Value);
					break;
				case Operation.Subtraction:
					stack.Push(-term.Value);
					break;
			}
		}

		return stack.Sum();
	}
}