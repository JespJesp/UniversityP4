using System.Runtime.CompilerServices;
using JespAst.Nodes.Melodies;
using JespAst.Nodes.Samples;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes;

public class ProgramNode(Node parent) : Node(parent)
{
	protected override void Parse()
	{
		while (Parser.CurrentToken.Type != TokenType.EndOfFile)
		{
			switch (Parser.CurrentToken.Type)
			{
				case TokenType.TimelineKeyword:; break;
				case TokenType.PatternKeyword:; break;
				case TokenType.MelodyKeyword: new MelodyDeclarationNode(this); break;
				case TokenType.SamplesKeyword: new SamplesNode(this); break;
				case TokenType.Newline: Parser.ConsumeToken(TokenType.Newline); break;
				default: throw new ArgumentOutOfRangeException();
			}
		}
	}
}

