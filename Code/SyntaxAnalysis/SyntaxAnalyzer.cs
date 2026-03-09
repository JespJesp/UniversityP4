using LexicalAnalysis;
using AST;
using SyntaxAnalysis.Parsers;

namespace SyntaxAnalysis;

public class SyntaxAnalyzer
{
	internal List<Token> Tokens = new();
	internal Song NewSong = new();
	internal int CursorPosition { get; private set; }
	internal Token CurrentToken() => Tokens[CursorPosition];

	public Song Parse(List<Token> inputTokens)
	{
		// Reset varaibles
		Tokens = inputTokens;
		CursorPosition = 0;
		NewSong = new Song();

		try
		{
			ParseRoots();
		}
		catch
		{
			throw new Exception($"Syntax error:\nUnexpected token: '{CurrentToken().ToString()}'.");
		}

		return NewSong;
	}

	private void ParseRoots()
	{
		// TODO: FIX
		while (Tokens.Count > 0)
		{
			switch (CurrentToken().Type)
			{
				case TokenType.KeywordTimeline: TimelineParser.Parse(this); break;
				case TokenType.Integer: PatternParser.Parse(this); break;
				case TokenType.NewLine: break;
				case TokenType.EndOfFile: break;
				default: throw new Exception();
			}
		}
	}

	#region Helper methods

	public void AdvanceCursor()
	{
		CursorPosition++;
	}

	/// <summary>
	/// Note: This also advances the cursor.
	/// </summary>
	public void ProcessToken(TokenType requiredTokenType, Action? actionBeforeAdvancingCursor = null)
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
	/// Note: This also advances the cursor.
	/// </summary>
	/// <returns> True if the next tokens are a new line followed by the required amount of tabs.</returns>
	public bool HasNewLineTabs(int requiredTabAmount)
	{
		// Check new line
		if (CurrentToken().Type != TokenType.NewLine)
		{
			return false;
		}
		AdvanceCursor();

		// Check tab amount
		int tabAmount = 0;
		for (int i = 0; i < requiredTabAmount; i++)
		{
			if (CurrentToken().Type != TokenType.Tab)
			{
				tabAmount++;
				AdvanceCursor();
			}
		}
		return tabAmount == requiredTabAmount;
	}

	#endregion
}
