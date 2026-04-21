using Tokens;
using Tokens.Tokenization;
using Tokens.Tokenization.Strategies;

namespace Phases.Lexing;

public class Lexer
{
	public List<Token> Tokens = new();
	public LexerCursor Cursor = new();

	private string _inputText = "";
	private List<string> _lexicalErrors = new();

	public char CursorChar => _inputText[Cursor.Position];
	public bool AtEndOfFile => Cursor.Position >= _inputText.Length;

	public List<Token> Lex(string text, string baseDirectory)
	{
		Tokens.Clear();
		_lexicalErrors.Clear();
		Cursor = new LexerCursor();
		_inputText = ExpandUsingStatements(text, baseDirectory, new HashSet<string>());

		LexInput();

		if (_lexicalErrors.Any())
		{
			throw new Exception("Lexical errors:\n- " + string.Join("\n- ", _lexicalErrors));
		}

		return Tokens;
	}

private static string ExpandUsingStatements(string text, string baseDirectory, HashSet<string> visitedFiles)
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
            string fullPath = Path.GetFullPath(Path.Combine(baseDirectory, path));

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
            string includedBaseDirectory = Path.GetDirectoryName(fullPath)!;
            string expandedContent = ExpandUsingStatements(fileContent, includedBaseDirectory, visitedFiles);

            result.Add(expandedContent);
        }
        else
        {
            result.Add(line);
        }
    }

    return string.Join("\n", result);
}
	private void LexInput()
	{
		// TODO: Add max size to e.g. float and string

		while (Cursor.Position < _inputText.Length)
		{
			try
			{
				if (!TryTokenizeChar())
				{
					Cursor.MoveToNextColumn();
					throw new LexicalException(Cursor.Line, Cursor.Column - 1, $"Unknown token type for character: '{CursorChar}'");
				}
			}
			catch (LexicalException exception)
			{
				_lexicalErrors.Add(exception.Message);
			}
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", Cursor.Line, Cursor.Column));
	}

	private bool TryTokenizeChar()
	{
		return Tokenizer.TryTokenize<WhitespaceStrategy>(this)
			|| Tokenizer.TryTokenize<CommentStrategy>(this)
			|| Tokenizer.TryTokenize<StringStrategy>(this)
			|| Tokenizer.TryTokenize<LeftParenthesesStrategy>(this)
			|| Tokenizer.TryTokenize<RightParenthesesStrategy>(this)
			|| Tokenizer.TryTokenize<CommaStrategy>(this)
			|| Tokenizer.TryTokenize<PlusStrategy>(this)
			|| Tokenizer.TryTokenize<AsteriskStrategy>(this)
			|| Tokenizer.TryTokenize<SlashStrategy>(this)
			|| Tokenizer.TryTokenize<NumberOrMinusStrategy>(this)
			|| Tokenizer.TryTokenize<IdentifierStrategy>(this);
	}
}