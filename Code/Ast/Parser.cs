using Ast.Nodes;
using Lexing.Tokens;

namespace Ast;

public static class Parser
{
	private static int _cursorPosition = 0;
	private static List<Token> _tokens = new();
	private static List<string> _syntaxErrors = new();

	public static Token CurrentToken => _tokens[_cursorPosition];

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

	public static void AddError(string errorMessage)
	{
		_syntaxErrors.Add($"Line: '{CurrentToken.Line}'. Column: '{CurrentToken.Column}'. Token type: '{CurrentToken.Type}'. Token value: '{CurrentToken.Value}'. {errorMessage}");

		// Skip everything on the line where the syntax error occurred
		while (CurrentToken.Type != TokenType.Newline)
		{
			_cursorPosition++;
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

		_cursorPosition++;

		return true;
	}

	public static bool TryConsumeTokens(Token[] requiredTokens)
	{
		bool isCorrectOrder = true;
		int lookahead = 0;
		foreach (Token requiredToken in requiredTokens)
		{
			Token lookaheadToken = _tokens[_cursorPosition + lookahead];
			if (!lookaheadToken.Type.IsSubtypeOf(requiredToken.Type) || lookaheadToken.Value != requiredToken.Value)
			{
				isCorrectOrder = false;
				break;
			}
			lookahead++;
		}

		if (isCorrectOrder)
		{
			_cursorPosition += lookahead;
			return true;
		}
		else
		{
			return false;
		}
	}

	public static bool TryConsumeIndent(int indentSize)
	{
		Token[] newlineAndIndent =
		{
			new Token(TokenType.Newline),
			new Token(TokenType.Indent, indentSize.ToString())
		};
		return TryConsumeTokens(newlineAndIndent);
	}

	public static void HandleUniqueOptions(Dictionary<TokenType, Action> options, Token[] separator)
	{
		List<TokenType> usedTokenTypes = new();
		do
		{
			if (options.TryGetValue(CurrentToken.Type, out Action? action))
			{
				if (usedTokenTypes.Contains(CurrentToken.Type))
				{
					throw new Exception($"Duplicate optional token '{CurrentToken.Type}'");
				}

				usedTokenTypes.Add(CurrentToken.Type);
				action();
			}
		} while (TryConsumeTokens(separator));
	}
}

