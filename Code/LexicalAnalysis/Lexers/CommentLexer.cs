namespace LexicalAnalysis.Lexers;

public static class CommentLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		a.Cursor.MoveToNextColumn(); // Skip opening hashtag
		while (a.CursorChar() != '#')
		{
			a.Cursor.MoveToNextColumn();
			if (!a.IsNotEndOfFile())
			{
				throw new Exception(
					$"String is missing closing quote '\"' at Line:{a.Cursor.Line} Column:{a.Cursor.Column}");
			}
		}
		a.Cursor.MoveToNextColumn(); // Skip closing hashtag
	}
}