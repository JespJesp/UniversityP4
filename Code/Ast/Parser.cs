using Ast.Nodes;
using Lexing.Tokens;

namespace Ast;

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

		if (_syntaxErrors.Any())
		{
			throw new Exception("Syntax errors:\n- " + string.Join("\n- ", _syntaxErrors));
		}

		return programNode;
	}

	public static void AddSyntaxError(string errorMessage)
	{
		_syntaxErrors.Add($"Line: '{CurrentToken.Line}'. Column: '{CurrentToken.Column}'. Token type: '{CurrentToken.Type}'. Token value: '{CurrentToken.Value}'. {errorMessage}");

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
		// TODO: Rewrite to make less messy

		if (CurrentToken.Type == TokenType.Newline)
		{
			Token lookaheadToken = _tokens[_cursorPosition + 1];

			if (lookaheadToken.Type == TokenType.Indent && lookaheadToken.Value == indentSize.ToString())
			{
				_cursorPosition += 2;
				return true;
			}
		}

		return false;
	}

	public static void HandleUniqueOptions(Dictionary<TokenType, Action> options, Token[] separator)
	{
		// TODO: Rewrite to make less messy

		List<TokenType> usedTokenTypes = new();
		TryConsumeNextOption();

		void TryConsumeNextOption()
		{
			if (options.TryGetValue(CurrentToken.Type, out Action? action))
			{
				if (usedTokenTypes.Contains(CurrentToken.Type))
				{
					throw new Exception($"Duplicate optional token '{CurrentToken.Type}'");
				}

				usedTokenTypes.Add(CurrentToken.Type);
				action();

				bool lacksSeparator = false;
				int lookahead = 0;
				foreach (Token separatorToken in separator)
				{
					Token lookaheadToken = _tokens[_cursorPosition + lookahead];
					if (!lookaheadToken.Type.IsSubtypeOf(separatorToken.Type) || lookaheadToken.Value != separatorToken.Value)
					{
						lacksSeparator = true;
						break;
					}
					lookahead++;
				}
				if (!lacksSeparator)
				{
					_cursorPosition += lookahead;
					TryConsumeNextOption();
				}
			}
		}
	}
}

