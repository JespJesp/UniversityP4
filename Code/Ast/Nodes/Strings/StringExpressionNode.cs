using Ast.NodeArchetypes;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Strings;

public class StringExpressionNode : BranchNode
{
	public string Value = "";

	internal List<Segment> _segments = new();

	internal class Segment
	{
		public string Value = "";
		public string RawValue = "";
		public bool IsIdentifier;
	};

	protected override void Parse()
	{
		do
		{
			Segment newSegment = new();

			newSegment.IsIdentifier = Parser.TryConsumeToken(TokenType.Identifier, (value) => newSegment.RawValue = value);
			if (!newSegment.IsIdentifier)
			{
				Parser.ConsumeToken(TokenType.String, (value) => newSegment.RawValue = value);
			}

			_segments.Add(newSegment);
		} while (Parser.TryConsumeToken(TokenType.Plus));
	}

	protected override void Annotate()
	{
		foreach (Segment segment in _segments)
		{
			if (segment.IsIdentifier)
			{
				if (!_symbolTable.Contains<StringConstantNode>(segment.RawValue))
				{
					throw new Exception($"String variable with ID '{segment.RawValue}' is not declared.");
				}

				segment.Value = _symbolTable.Get<StringConstantNode>(segment.RawValue).StringExpression.Value;
			}
			else
			{
				segment.Value = segment.RawValue;
			}

			this.Value += segment.Value;
		}
	}
}