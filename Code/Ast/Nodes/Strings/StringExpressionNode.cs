using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Strings;

public class StringExpressionNode : BranchNode
{
	public string Value = "";

	internal List<Segment> _segments = new();

	internal class Segment
	{
		public string Value = "";
		public string StringOrIdentifierValue = "";
		public bool IsIdentifier;
	};

	protected override void Parse()
	{
		do
		{
			Segment newSegment = new();

			newSegment.IsIdentifier = Parser.TryConsumeToken(TokenType.Identifier, (value) => newSegment.StringOrIdentifierValue = value);
			if (!newSegment.IsIdentifier)
			{
				Parser.ConsumeToken(TokenType.String, (value) => newSegment.StringOrIdentifierValue = value);
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
				if (!_symbolTable.Contains<StringConstantNode>(segment.StringOrIdentifierValue))
				{
					throw new Exception($"String variable with ID '{segment.StringOrIdentifierValue}' is not declared.");
				}

				segment.Value = _symbolTable.Get<StringConstantNode>(segment.StringOrIdentifierValue).StringExpression.Value;
			}
			else
			{
				segment.Value = segment.StringOrIdentifierValue;
			}

			this.Value += segment.Value;
		}
	}
}