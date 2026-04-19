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
		while (CursorToken.Type != TokenType.Newline)
		{
			_cursorPosition++;
		}
	}

	public void ConsumeToken(TokenType requiredType, string requiredValue, Action<string>? useValue = null)
	{
		if (TryConsumeToken(requiredType, requiredValue, useValue) == false)
		{
			throw new Exception($"Expected token of type '{requiredType}' and value '{requiredValue}'");
		}
	}
	public void ConsumeToken(TokenType requiredType, Action<string>? useValue = null)
	{
		if (TryConsumeToken(requiredType, useValue) == false)
		{
			throw new Exception($"Expected token of type '{requiredType}'");
		}
	}

	public bool TryConsumeToken(TokenType requiredType, string requiredValue, Action<string>? useValue = null)
	{
		if (CursorToken.Value != requiredValue)
		{
			return false;
		}
		else
		{
			return TryConsumeToken(requiredType, useValue);
		}
	}
	public bool TryConsumeToken(TokenType requiredType, Action<string>? useValue = null)
	{
		if (!CursorToken.Type.IsSubtypeOf(requiredType))
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
	public void TryConsumeOptions(List<Func<bool>> tryConsumeTokenMethods, Token[] separator)
	{
		List<Func<bool>> unusedTryConsumeTokenMethods = new(tryConsumeTokenMethods);
		while (unusedTryConsumeTokenMethods.Count != 0)
		{
			foreach (Func<bool> tryConsumeTokenMethod in unusedTryConsumeTokenMethods)
			{
				if (tryConsumeTokenMethod())
				{
					unusedTryConsumeTokenMethods.Remove(tryConsumeTokenMethod);

					if (!TryConsumeTokens(separator))
					{
						return;
					}
				}
			}
		}
	}
}

