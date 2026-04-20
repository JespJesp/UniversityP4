using Ast;
using Ast.Nodes;
using Tokens;

namespace Phases.Parsing;

public class Parser
{
	private int _cursorPosition = 0;
	private List<Token> _tokens = new();
	private List<string> _errors = new();

	public Token CursorToken => _tokens[_cursorPosition];

	public ProgramNode Parse(List<Token> inputTokens)
	{
		_tokens = inputTokens;

		ProgramNode astRoot = new ProgramNode();
		astRoot.CascadeParse(this);

		if (_errors.Any())
		{
			throw new Exception("Syntax errors:\n- " + string.Join("\n- ", _errors));
		}

		return astRoot;
	}

	public T ParseChild<T>(Node parent, T newChild, bool createsNestedScope = false) where T : Node
	{
		// Assign child properties
		newChild.CreatesNestedScope = createsNestedScope;
		newChild.Column = CursorToken.Column;
		newChild.Line = CursorToken.Line;
		if (parent.CreatesNestedScope)
		{
			newChild.ScopeDepth = parent.ScopeDepth + 1;
		}
		else
		{
			newChild.ScopeDepth = parent.ScopeDepth;
		}
		parent.Children.Add(newChild);

		// Parse child
		try
		{
			newChild.CascadeParse(this);
		}
		catch (Exception exception)
		{
			AddErrorAndSkipLine(newChild, exception.Message);
		}

		return newChild;
	}

	public void AddErrorAndSkipLine(Node node, string errorMessage)
	{
		_errors.Add($"Line: '{CursorToken.Line}'. Column: '{CursorToken.Column}'. Token type: '{CursorToken.Type}'. Token value: '{CursorToken.Value}'. Node type: '{node.GetType()}'. {errorMessage}");

		// Skip everything on the line where the syntax error occurred
		// because the error will likely impact the whole line
		while (CursorToken.Type != TokenType.EndOfFile && CursorToken.Type != TokenType.Newline)
		{
			_cursorPosition++;
		}
	}

	public bool TryConsumeToken(TokenType requiredType, out string tokenValue)
	{
		if (!CursorToken.Type.IsSubtypeOf(requiredType))
		{
			tokenValue = "";
			return false;
		}

		tokenValue = CursorToken.Value;

		_cursorPosition++;

		return true;
	}
	public bool TryConsumeToken(TokenType requiredType, string requiredValue, out string tokenValue)
	{
		if (CursorToken.Value != requiredValue)
		{
			tokenValue = "";
			return false;
		}
		else
		{
			return TryConsumeToken(requiredType, out tokenValue);
		}
	}
	public bool TryConsumeToken(TokenType requiredType, string requiredValue)
	{
		return TryConsumeToken(requiredType, requiredValue, out string ignoredTokenValue);
	}
	public bool TryConsumeToken(TokenType requiredType)
	{
		return TryConsumeToken(requiredType, out string ignoredTokenValue);
	}

	public void ConsumeToken(TokenType requiredType, out string tokenValue)
	{
		if (TryConsumeToken(requiredType, out tokenValue) == false)
		{
			throw new Exception($"Expected token of type '{requiredType}'");
		}
	}
	public void ConsumeToken(TokenType requiredType, string requiredValue, out string tokenValue)
	{
		if (TryConsumeToken(requiredType, requiredValue, out tokenValue) == false)
		{
			throw new Exception($"Expected token of type '{requiredType}' and value '{requiredValue}'");
		}
	}
	public void ConsumeToken(TokenType requiredType, string requiredValue)
	{
		ConsumeToken(requiredType, requiredValue, out string ignoredTokenValue);
	}
	public void ConsumeToken(TokenType requiredType)
	{
		ConsumeToken(requiredType, out string ignoredTokenValue);
	}

	public bool TryConsumeTokens(Token[] requiredTokens)
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

	public bool TryConsumeIndent(int indentSize)
	{
		Token[] newlineAndIndent =
		{
			new Token(TokenType.Newline),
			new Token(TokenType.Indent, indentSize.ToString())
		};
		return TryConsumeTokens(newlineAndIndent);
	}

	/// <summary>
	/// Note: Options are allowed to appear in any order.
	/// </summary>
	public void TryConsumeOptions(List<(Func<bool> tryConsumeToken, Action afterConsumption)> options, Token[] separator)
	{
		List<(Func<bool> tryConsumeToken, Action afterConsumption)> unusedOptions = new(options);

		TryUseOption();

		void TryUseOption()
		{
			foreach (var option in unusedOptions)
			{
				if (option.tryConsumeToken())
				{
					option.afterConsumption();
					unusedOptions.Remove(option);
					if (TryConsumeTokens(separator))
					{
						TryUseOption();
					}
					return;
				}
			}
		}
	}
}

