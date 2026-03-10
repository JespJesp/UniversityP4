namespace LexicalAnalysis.Lexers;

public static class CommentLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		a.AdvanceCursorToNextColumn(); // Skip opening ashtag
		while (a.CursorChar() != '#')
		{
			a.AdvanceCursorToNextColumn();
			if (!a.IsNotEndOfFile())
			{
				throw new Exception(
					$"String is missing closing quote '\"' at Line:{a.CursorLine} Column:{a.CursorColumn}");
			}
		}
		a.AdvanceCursorToNextColumn(); // Skip closing hashtag
	}
}