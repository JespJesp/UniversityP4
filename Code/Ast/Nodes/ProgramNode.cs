using Ast.Nodes.Floats;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;
using Ast.Nodes.Samples;
using Ast.Nodes.Strings;
using Ast.Nodes.Timelines;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes;

public class ProgramNode : Node
{
	public override void CascadeParse(Parser parser)
	{
		while (parser.CursorToken.Type != TokenType.EndOfFile)
		{
			switch (parser.CursorToken.Type)
			{
				case TokenType.TimelineKeyword: parser.ParseChild(this, new TimelineNode()); break;
				case TokenType.PatternKeyword: parser.ParseChild(this, new PatternNode()); break;
				case TokenType.MelodyKeyword: parser.ParseChild(this, new MelodyNode()); break;
				case TokenType.SampleKeyword: parser.ParseChild(this, new SampleNode()); break;
				case TokenType.StringKeyword: parser.ParseChild(this, new StringConstantNode()); break;
				case TokenType.FloatKeyword: parser.ParseChild(this, new FloatConstantNode()); break;
				case TokenType.Newline: parser.ConsumeToken(TokenType.Newline); break;
				default: throw new ArgumentOutOfRangeException($"Unexpected token");
			}
		}
	}
}

