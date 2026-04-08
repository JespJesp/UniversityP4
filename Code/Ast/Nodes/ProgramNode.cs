using System.Runtime.CompilerServices;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;
using Ast.Nodes.Samples;
using Ast.Nodes.Timelines;
using Ast.Tables;
using Lexing.Tokens;

namespace Ast.Nodes;

public class ProgramNode(Node parent = null, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	protected override void Parse()
	{
		while (Parser.CurrentToken.Type != TokenType.EndOfFile)
		{
			switch (Parser.CurrentToken.Type)
			{
				case TokenType.TimelineKeyword: new TimelineNode(this); break;
				case TokenType.PatternKeyword: new PatternNode(this); break;
				case TokenType.MelodyKeyword: new MelodyNode(this); break;
				case TokenType.SampleKeyword: new SampleNode(this); break;
				case TokenType.Newline: Parser.ConsumeToken(TokenType.Newline); break;
				default: throw new ArgumentOutOfRangeException($"Unexpected token");
			}
		}
	}
}

