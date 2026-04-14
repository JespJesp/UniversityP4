using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Strings;

public class StringExpressionNode : BranchNode
{
	internal class Segment
	{
		public string Value = "";
		public string StringOrIdentifierValue = "";
		public bool IsIdentifier;
	};
	internal List<Segment> _segments = new();
	public string Value = "";

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
				if (!_symbolTable.Contains<StringDeclarationNode>(segment.StringOrIdentifierValue))
				{
					Annotator.AddError(this, $"String variable with ID '{segment.StringOrIdentifierValue}' is not declared.");
				}

				segment.Value = _symbolTable.Get<StringDeclarationNode>(segment.StringOrIdentifierValue).StringExpression.Value;
			}
			else
			{
				segment.Value = segment.StringOrIdentifierValue;
			}

			this.Value += segment.Value;
		}
	}
}