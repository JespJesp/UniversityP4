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

public class ProgramNode : RootNode
{
	protected override void Parse()
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
						ParseChild(new TimelineNode());
					}
					break;
				case TokenType.PatternKeyword: ParseChild(new PatternNode()); break;
				case TokenType.MelodyKeyword: ParseChild(new MelodyNode()); break;
				case TokenType.SampleKeyword: ParseChild(new SampleNode()); break;
				case TokenType.StringKeyword: ParseChild(new StringConstantNode()); break;
				case TokenType.FloatKeyword: ParseChild(new FloatConstantNode()); break;
				case TokenType.Newline: Parser.ConsumeToken(TokenType.Newline); break;
				default: throw new ArgumentOutOfRangeException($"Unexpected token");
			}
		}
	}
}

