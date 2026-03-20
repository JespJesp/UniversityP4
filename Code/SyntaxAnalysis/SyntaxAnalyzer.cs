using LexicalAnalysis;
using AST;
using SyntaxAnalysis.Parsers;

namespace SyntaxAnalysis;

public class SyntaxAnalyzer
{
	private List<Token> _tokens = new();
	private SyntaxAnalyzerCursor _cursor = new();

	public Song OutputSong = new();
	public Token CursorToken() => _tokens[_cursor.Position];

	public Song Parse(List<Token> inputTokens)
	{
		// Reset variables
		_tokens = inputTokens;
		_cursor.MoveToStartPosition();
		OutputSong = new Song();

		try
		{
			ParseRoots();
		}
		catch
		{
			throw new Exception($"Syntax error:\n- Unexpected token: '{CursorToken().ToString()}'.");
		}

		return OutputSong;
	}

	private void ParseRoots()
	{
		while (!HasConsumedAllTokens())
		{
			switch (CursorToken().Type)
			{
				case TokenType.TimelineKeyword: TimelineParser.Parse(this); break;
				case TokenType.Integer: PatternParser.Parse(this); break;
				case TokenType.NewLine: ConsumeToken(TokenType.NewLine); break;
				case TokenType.EndOfFile: ConsumeToken(TokenType.EndOfFile); break;
				default: throw new Exception();
			}
		}
	}

	public bool HasConsumedAllTokens() => _cursor.Position >= _tokens.Count;

	/// <summary>
	/// Moves cursor to next token, if current token is a specific type (and throws an exception if it is not). 
	/// Can optionally perform an action before moving the cursor.
	/// </summary>
	public void ConsumeToken(TokenType requiredTokenType, Action? actionBeforeAdvancingCursor = null)
	{
		if (CursorToken().Type != requiredTokenType)
		{
			throw new Exception();
		}

		if (actionBeforeAdvancingCursor != null)
		{
			actionBeforeAdvancingCursor();
		}

		_cursor.MoveToNextToken();
	}

	/// <summary>
	/// Tries to consume the next tokens if they are a new line followed by a specific amount of tabs.
	/// </summary>
	/// <returns> True on success.</returns>
	public bool TryConsumeNewLineAndTabs(int requiredTabAmount)
	{
		int cursorLookahead = 0;
		Token LookaheadToken() => _tokens[_cursor.Position + cursorLookahead];

		// Check new line is present
		if (LookaheadToken().Type != TokenType.NewLine)
		{
			return false;
		}
		cursorLookahead++;

		// Check tabs are present
		int tabAmount = 0;
		for (int i = 0; i < requiredTabAmount; i++)
		{
			if (LookaheadToken().Type == TokenType.Tab)
			{
				tabAmount++;
				cursorLookahead++;
				continue;
			}
			break;
		}
		if (tabAmount != requiredTabAmount)
		{
			return false;
		}

		// Consume new line and tabs
		for (int i = 0; i < cursorLookahead; i++)
		{
			_cursor.MoveToNextToken();
		}
		return true;
	}
}
