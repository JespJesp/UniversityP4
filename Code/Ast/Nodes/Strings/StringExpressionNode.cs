using Phases.Annotation;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Strings;

public class StringExpressionNode : Node
{
	public string Value = "";

	internal List<Segment> _segments = new();

	internal class Segment
	{
		public string Value = "";
		public string RawValue = "";
		public bool IsIdentifier;
	};

	public override void CascadeParse(Parser parser)
	{
		do
		{
			Segment newSegment = new();
			newSegment.IsIdentifier = parser.TryConsumeToken(TokenType.Identifier, out newSegment.RawValue);
			if (!newSegment.IsIdentifier)
			{
				parser.ConsumeToken(TokenType.String, out newSegment.RawValue);
			}
			_segments.Add(newSegment);
		} while (parser.TryConsumeToken(TokenType.Plus));
	}

	public override void Annotate(Annotator annotator)
	{
		foreach (Segment segment in _segments)
		{
			if (segment.IsIdentifier)
			{
				if (!SymbolTable.Contains<StringConstantNode>(segment.RawValue))
				{
					throw new Exception($"String variable with ID '{segment.RawValue}' is not declared.");
				}

				segment.Value = SymbolTable.Get<StringConstantNode>(segment.RawValue).StringExpression.Value;
			}
			else
			{
				segment.Value = segment.RawValue;
			}

			this.Value += segment.Value;
		}
	}
}