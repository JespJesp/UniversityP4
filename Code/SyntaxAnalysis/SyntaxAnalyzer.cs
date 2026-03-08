using LexicalAnalysis;
using Song;
using SyntaxAnalysis.Parsing;
using Visitation;

namespace SyntaxAnalysis;

public class SyntaxAnalyzer
{
	public Stack<IVisitable> VisitableStack = new Stack<IVisitable>();
	public Queue<Token> Tokens = new();
	public Pattern CurrentPattern = new();
	public Token CurrentToken;

	public Pattern Analyze(List<Token> input)
	{
		Tokens = new Queue<Token>(input);
		CurrentPattern = new();

		ParseTokens();

		return CurrentPattern;
	}

	private void ParseTokens()
	{
		while (Tokens.Count > 0)
		{
			CurrentToken = Tokens.Dequeue();

			switch (CurrentToken.Type)
			{
				case TokenType.Number: ParseNumber.Parse(this); break;
				case TokenType.Identifier: ParseIdentifier.Parse(this); break;
				case TokenType.String: ParseString.Parse(this); break;
				case TokenType.SamplesKeyword: ParseSample.Parse(this); break;
				case TokenType.NotesKeyword: break;
				case TokenType.Hyphen: ParseHyphen.Parse(this); break;
				case TokenType.NewLine: break;
				case TokenType.EndOfFile: break;
				default:
					throw new NotImplementedException(
						$"Error while parsing: Token type not implented for token: '{CurrentToken.ToString()}'");
			}
		}
	}
}
