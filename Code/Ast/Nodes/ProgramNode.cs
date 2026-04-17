using Ast.NodeArchetypes;
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
	public override void CascadeParse()
	{
		bool hasConsumedTimelineKeyword = false;

		while (Parser.CursorToken.Type != TokenType.EndOfFile)
		{
			switch (Parser.CursorToken.Type)
			{
				case TokenType.TimelineKeyword:
					if (hasConsumedTimelineKeyword)
					{
						Parser.AddErrorAndSkipLine(this, "'timeline' keyword appears multiple times.");
					}
					else
					{
						hasConsumedTimelineKeyword = true;
						Parser.ParseChild(this, new TimelineNode());
					}
					break;
				case TokenType.PatternKeyword: Parser.ParseChild(this, new PatternNode()); break;
				case TokenType.MelodyKeyword: Parser.ParseChild(this, new MelodyNode()); break;
				case TokenType.SampleKeyword: Parser.ParseChild(this, new SampleNode()); break;
				case TokenType.StringKeyword: Parser.ParseChild(this, new StringConstantNode()); break;
				case TokenType.FloatKeyword: Parser.ParseChild(this, new FloatConstantNode()); break;
				case TokenType.Newline: Parser.ConsumeToken(TokenType.Newline); break;
				default: throw new ArgumentOutOfRangeException($"Unexpected token");
			}
		}
	}
}

