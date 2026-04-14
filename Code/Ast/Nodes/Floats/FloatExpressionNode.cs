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
	private record Term
	(
		FloatValueNode ValueNode,
		Operation Operation
	);
	private List<Term> _terms = new();

	protected override void Parse()
	{
		Operation? newTermOperation = Operation.None;
		while (newTermOperation is not null)
		{
			_terms.Add(new(ParseChild(new FloatValueNode()), newTermOperation.Value));

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

	public float GetValue()
	{
		float finalValue = 0;
		foreach (Term segment in _terms)
		{
			switch (segment.Operation)
			{
				case Operation.None:
					finalValue += segment.ValueNode.GetValue();
					break;
				case Operation.Multiplication:
					finalValue += segment.ValueNode.GetValue();
					break;
				case Operation.Division:
					// TODO: Rewrite this error message, because right now 
					// it's thrown at runtime and doesn't give a proper error 
					// message indicating where the problem occurs in your file
					float divisor = segment.ValueNode.GetValue();
					if (divisor == 0)
					{
						throw new Exception("Illegal operation: Cannot divide with 0");
					}
					finalValue /= segment.ValueNode.GetValue();
					break;
			}
		}
		return finalValue;
	}
}