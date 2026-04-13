using Ast.NodeArchetypes;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;
using Ast.Nodes.Samples;
using Ast.Nodes.Strings;
using Ast.Nodes.Timelines;
using Lexing.Tokens;

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
						Parser.AddError($"Node type: {this.GetType()}. 'timeline' keyword appears multiple times.");
					}
					else
					{
						hasConsumedTimelineKeyword = true;
						new TimelineNode(this);
					}
					break;
				case TokenType.PatternKeyword: new PatternNode(this); break;
				case TokenType.MelodyKeyword: new MelodyNode(this); break;
				case TokenType.SampleKeyword: new SampleNode(this); break;
				case TokenType.StringKeyword: new StringDeclarationNode(this); break;
				case TokenType.Newline: Parser.ConsumeToken(TokenType.Newline); break;
				default: throw new ArgumentOutOfRangeException($"Unexpected token");
			}
		}
	}
}

