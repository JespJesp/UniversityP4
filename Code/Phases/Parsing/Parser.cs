using Ast.NodeArchetypes;
using Ast.Nodes;
using Tokens;

namespace Phases.Parsing;

public static class Parser
{
	private static int _cursorPosition = 0;
	private static List<Token> _tokens = new();
	private static List<string> _errors = new();

	public static Token CursorToken => _tokens[_cursorPosition];

	public static ProgramNode Parse(List<Token> inputTokens)
	{
		_tokens = inputTokens;

		ProgramNode astRoot = new ProgramNode();
		astRoot.ParseTree();

		if (_errors.Any())
		{
			throw new Exception("Syntax errors:\n- " + string.Join("\n- ", _errors));
		}

		return astRoot;
	}

	public static void AddErrorAndSkipLine(Node node, string errorMessage)
	{
		_errors.Add($"Line: '{CursorToken.Line}'. Column: '{CursorToken.Column}'. Token type: '{CursorToken.Type}'. Token value: '{CursorToken.Value}'. Node type: '{node.GetType()}'. {errorMessage}");

		// Skip everything on the line where the syntax error occurred
		// because the error will likely impact the whole line
		while (CursorToken.Type != TokenType.Newline)
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
		if (!CursorToken.Type.IsSubtypeOf(required))
		{
			return false;
		}

		if (useValue is not null)
		{
			useValue(CursorToken.Value);
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

	/// <summary>
	/// Tries to consume TokenTypes and execute Actions
	/// as specified in each "(TokenType, Action)" option pair.
	/// Each option is separated by a separator (e.g. a comma Token).
	/// Each option's token type: 
	/// 1) is not required to appear, thereby making it "optional",
	/// 2) may only appear once, thereby making it "unique",
	/// 3) may appear in a random order.
	/// </summary>
	public static void AllowUniqueOptions(Dictionary<TokenType, Action> options, Token[] separator)
	{
		List<TokenType> usedTokenTypes = new();
		do
		{
			if (options.TryGetValue(CursorToken.Type, out Action? action))
			{
				if (usedTokenTypes.Contains(CursorToken.Type))
				{
					throw new Exception($"Duplicate optional token '{CursorToken.Type}'");
				}

				usedTokenTypes.Add(CursorToken.Type);
				action();
			}
		} while (TryConsumeTokens(separator));
	}
}

