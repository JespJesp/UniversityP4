using JespAst.Nodes;
using LexicalAnalysis.Tokens;

namespace JespAst;

public static class Parser
{
	private static int _cursorPosition = 0;
	private static List<Token> _tokens = new();
	private static List<string> _syntaxErrors = new();

	public static Token CurrentToken => _tokens[_cursorPosition];
	private static void AdvanceCursor() => _cursorPosition++;

	public static ProgramNode ParseTree(List<Token> inputTokens)
	{
		_tokens = inputTokens;
		_cursorPosition = 0;

		ProgramNode programNode = new ProgramNode();
		programNode.CascadeParse();

		if (_syntaxErrors.Any())
		{
			throw new Exception("Syntax errors:\n" + string.Join("\n- ", _syntaxErrors));
		}

		return programNode;
	}

	public static void AddSyntaxError(string errorMessage)
	{
		_syntaxErrors.Add($"Line: {CurrentToken.Line}. Column: {CurrentToken.Column}. Token type: {CurrentToken.Type}. {errorMessage}");

		// Skip everything on the line where the syntax error occurred
		while (CurrentToken.Type != TokenType.Newline)
		{
			AdvanceCursor();
		}
	}

	public static void ConsumeToken(TokenType required, Action<string>? useValue = null)
	{
		if (TryConsumeToken(required, useValue) == false)
		{
			throw new Exception($"Expected token of type '{required}'");
		}
	}

	public static bool TryConsumeToken(TokenType required, Action<string>? useValue = null)
	{
		if (!CurrentToken.Type.IsSubtypeOf(required))
		{
			return false;
		}

		if (useValue is not null)
		{
			useValue(CurrentToken.Value);
		}

		AdvanceCursor();

		return true;
	}

	public static bool TryConsumeIndent(int indentSize)
	{
		if (CurrentToken.Type == TokenType.Newline)
		{
			AdvanceCursor();

			if (CurrentToken.Type == TokenType.Indent && CurrentToken.Value == indentSize.ToString())
			{
				ConsumeToken(TokenType.Indent);
				return true;
			}
		}

		return false;
	}

	public static void TryConsumeUniqueOptions(Dictionary<TokenType, Action> options, Token? separator = null)
	{
		List<TokenType> usedTokenTypes = new();
		TryConsumeNextOption();

		void TryConsumeNextOption()
		{
			if (options.TryGetValue(CurrentToken.Type, out Action? actionAfterConsumption))
			{
				if (usedTokenTypes.Contains(CurrentToken.Type))
				{
					throw new Exception($"Duplicate optional token '{CurrentToken.Type}'");
				}

				usedTokenTypes.Add(CurrentToken.Type);
				ConsumeToken(CurrentToken.Type);
				actionAfterConsumption();

				if (separator is null
					|| separator is not null && CurrentToken.Type.IsSubtypeOf(separator.Type) && CurrentToken.Value == separator.Value)
				{
					TryConsumeNextOption();
				}
			}
		}
	}
}

