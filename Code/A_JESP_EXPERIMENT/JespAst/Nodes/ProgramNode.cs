using System.Runtime.CompilerServices;
using JespAst.Nodes.Melodies;
using JespAst.Nodes.Patterns;
using JespAst.Nodes.Samples;
using JespAst.Nodes.Timelines;
using JespAst.Tables;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes;

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
				default: throw new ArgumentOutOfRangeException($"Unexcepted token");
			}
		}
	}
}

