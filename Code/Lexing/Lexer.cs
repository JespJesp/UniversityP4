using Lexing.Lexers;
using Lexing.Tokens;

namespace Lexing;

public static class Lexer
{
	private static string _inputText = "";
	private static List<LexicalError> _errors = new();

	public static List<Token> Tokens = new();
	public static LexicalAnalyzerCursor Cursor = new();

	public static char CursorChar => _inputText[Cursor.Position];
	public static bool IsNotEndOfFile => Cursor.Position < _inputText.Length;

	public static List<Token> Lex(string text)
	{
		// Reset variables
		Tokens.Clear();
		_errors.Clear();
		_inputText = ExpandUsingStatements(text, new HashSet<string>());
		Cursor.MoveToStartPosition();

		LexText();

		if (_errors.Any())
		{
			string errorMessage = "Lexical errors:";
			foreach (LexicalError error in _errors)
			{
				errorMessage += $"\n- Line: {error.Line}, Column: {error.Column}, Message: {error.Message}";
			}
			throw new Exception(errorMessage);
		}

		return Tokens;
	}

	private static void LexText()
	{
		// TODO: Add max size to e.g. float and string

		while (Cursor.Position < _inputText.Length)
		{
			if (char.IsWhiteSpace(CursorChar))
			{
				WhitespaceLexer.Lex();
			}
			else if (CursorChar == '"')
			{
				StringLexer.Lex();
			}
			else if (CursorChar == '(')
			{
				Tokens.Add(new Token(TokenType.LeftParentheses, "", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar == ')')
			{
				Tokens.Add(new Token(TokenType.RightParentheses, "", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar == ',')
			{
				Tokens.Add(new Token(TokenType.Comma, "", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar == '-' || char.IsDigit(CursorChar))
			{
				NumberLexer.Lex();
			}
			else if (CursorChar == '_' || CursorChar == '#' || char.IsLetter(CursorChar))
			{
				IdentifierOrKeywordLexer.Lex();
			}
			else if (CursorChar == '%')
			{
				CommentLexer.Lex();
			}
			else
			{
				_errors.Add(new LexicalError(Cursor.Column, Cursor.Line, $"Unknown token type: Character: '{CursorChar}'"));
				Cursor.MoveToNextColumn();
			}
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", Cursor.Line, Cursor.Column));
	}
private static string ExpandUsingStatements(string text, HashSet<string> visitedFiles)
{
    var lines = text.Split('\n');
    var result = new List<string>();

    foreach (var line in lines)
    {
        var trimmed = line.Trim();

        if (trimmed.StartsWith("using "))
        {
            int firstQuote = trimmed.IndexOf('"');
            int lastQuote = trimmed.LastIndexOf('"');

            if (firstQuote == -1 || lastQuote == -1 || lastQuote <= firstQuote)
            {
                throw new Exception($"Invalid using statement: {line}");
            }

            string path = trimmed.Substring(firstQuote + 1, lastQuote - firstQuote - 1);

            string fullPath = Path.GetFullPath(path);

            if (visitedFiles.Contains(fullPath))
            {
                throw new Exception($"Circular using detected: {fullPath}");
            }

            if (!File.Exists(fullPath))
            {
                throw new Exception($"Could not find file: {fullPath}");
            }

            visitedFiles.Add(fullPath);

            string fileContent = File.ReadAllText(fullPath);

            string expandedContent = ExpandUsingStatements(fileContent, visitedFiles);

            result.Add(expandedContent);
        }
        else
        {
            result.Add(line);
        }
    }

    return string.Join("\n", result);
}
	public static void AddError(LexicalError error)
	{
		_errors.Add(error);
	}
}