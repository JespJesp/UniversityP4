using Ast.Nodes.Floats;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;
using Ast.Nodes.Samples;
using Ast.Nodes.Strings;
using Ast.Nodes.Timelines;
using Phases.Annotation;
using Phases.Parsing;
using Phases.Validation;
using Tokens;

namespace Ast.Nodes;

public class ProgramNode : Node
{
	public TimelineNode timelineNode = new();

	public override void CascadeParse(Parser parser)
	{
		while (parser.CursorToken.Type != TokenType.EndOfFile)
		{
			if(parser.TryConsumeToken(TokenType.Identifier, "timeline", (value) =>
				{
					timelineNode = parser.ParseChild(this, new TimelineNode());
				})
				|| parser.TryConsumeToken(TokenType.Identifier, "pattern", (value) =>
				{
					parser.ParseChild(this, new PatternNode());
				})
				|| parser.TryConsumeToken(TokenType.Identifier, "melody", (value) =>
				{
					parser.ParseChild(this, new MelodyNode());
				})
				|| parser.TryConsumeToken(TokenType.Identifier, "sample", (value) =>
				{
					parser.ParseChild(this, new SampleNode());
				})
				|| parser.TryConsumeToken(TokenType.Identifier, "string", (value) =>
				{
					parser.ParseChild(this, new StringConstantNode());
				})
				|| parser.TryConsumeToken(TokenType.Identifier, "float", (value) =>
				{
					parser.ParseChild(this, new FloatConstantNode());
				})
				|| parser.TryConsumeToken(TokenType.Newline, (value) =>
				{
					parser.ConsumeToken(TokenType.Newline);
				}))
			{
				throw new Exception($"Unexpected token");
			}
		}
	}
}

