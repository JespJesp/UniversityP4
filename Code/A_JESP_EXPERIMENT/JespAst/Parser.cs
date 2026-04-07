using LexicalAnalysis.Tokens;

namespace JespAst;

public static class Parser
{
	public static Token CurrentToken;

	private static void AdvanceCursor()
	{
		// TODO: Implement
	}

	public static void AddError()
	{
		// TODO: Implement

		// TODO: If there is an error on a line, skip that line (so consume every token until you reach a newline token)
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

