using LexicalAnalysis;
using AST;
using SyntaxAnalysis.Parsers;

namespace SyntaxAnalysis;

public class SyntaxAnalyzer
{
	private List<Token> _tokens = new();
	private int _cursorPosition;

	public Song OutputSong = new();
	public Token CurrentToken() => _tokens[_cursorPosition];

	public Song Parse(List<Token> inputTokens)
	{
		// Reset variables
		_tokens = inputTokens;
		_cursorPosition = 0;
		OutputSong = new Song();

		try
		{
			ParseRoots();
		}
		catch
		{
			throw new Exception($"Syntax error:\n- Unexpected token: '{CurrentToken().ToString()}'.");
		}

		return OutputSong;
	}

	private void ParseRoots()
	{
		while (!HasProcessedAllTokens())
		{
			switch (CurrentToken().Type)
			{
				case TokenType.TimelineKeyword: TimelineParser.Parse(this); break;
				case TokenType.Integer: PatternParser.Parse(this); break;
				case TokenType.NewLine: ConsumeToken(TokenType.NewLine); break;
				case TokenType.EndOfFile: ConsumeToken(TokenType.EndOfFile); break;
				default: throw new Exception();
			}
		}
	}

	public bool HasProcessedAllTokens() => _cursorPosition >= _tokens.Count;

	/// <summary>
	/// Note: This advances the cursor.
	/// </summary>
	public void ConsumeToken(TokenType requiredTokenType, Action? actionBeforeAdvancingCursor = null)
	{
		if (CurrentToken().Type != requiredTokenType)
		{
			throw new Exception();
		}

		if (actionBeforeAdvancingCursor != null)
		{
			actionBeforeAdvancingCursor();
		}

		AdvanceCursor();
	}

	/// <summary>
	/// Note: If this returns true, it also advances the cursor to consume the new line and tabs.
	/// </summary>
	/// <returns> True if the next tokens are a new line followed by the required amount of tabs.</returns>
	public bool TryConsumeNewLineAndTabs(int requiredTabAmount)
	{
		int cursorLookahead = 0;
		Token LookaheadToken() => _tokens[_cursorPosition + cursorLookahead];

		// Check new line
		if (LookaheadToken().Type != TokenType.NewLine)
		{
			return false;
		}
		cursorLookahead++;

		// Check tabs
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
		_cursorPosition += cursorLookahead;
		return true;
	}

	private void AdvanceCursor()
	{
		_cursorPosition++;
	}
}
