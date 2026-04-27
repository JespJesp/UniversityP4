using Tokens;

namespace Phases.Lexing;

public class Lexer
{
	public List<Token> Tokens = new();
	internal List<string> ImportedFilePaths = new();
	internal string BaseDirectory = "";
	internal List<string> Errors = new();

	public List<Token> Lex(string baseFileContent, string baseFileName, string baseDirectoryPath)
	{
		Errors.Clear();
		Tokens.Clear();

		BaseDirectory = baseDirectoryPath;
		var baseFile = new FileLexer(this, baseFileName, baseFileContent);

		baseFile.Lex();

		if (Errors.Any())
		{
			throw new Exception("Lexical errors:\n- " + string.Join("\n- ", Errors));
		}

		return Tokens;
	}
}