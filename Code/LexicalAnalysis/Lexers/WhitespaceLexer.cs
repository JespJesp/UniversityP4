using LexicalAnalysis.Tokens;

namespace LexicalAnalysis.Lexers;

public static class WhitespaceLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		if (a.CursorChar() == '\n')
		{
			a.Tokens.Add(new Token(TokenType.Newline, "\n", a.Cursor.Line, a.Cursor.Column));
			a.Cursor.MoveToNewLine();

			int indentSize = 0;
			while (a.CursorChar() == '\t')
			{
				indentSize++;
				a.Cursor.MoveToNextColumn();
			}

			if (indentSize != 0)
			{
				a.Tokens.Add(new Token(TokenType.Indent, indentSize.ToString(), a.Cursor.Line, a.Cursor.Column));
			}
		}
		else // Ignore whitespace if not a newline
		{
			a.Cursor.MoveToNextColumn();
		}
	}
}