using Tokens;

namespace Phases.Lexing;

public class Lexer
{
	public List<Token> Tokens = new();
	internal List<string> ImportedFileFullPaths = new();
	internal string InputFileFolderFullPath = "";
	internal List<string> Errors = new();

	public List<Token> Lex(string fileContent, FileInfo fileInfo)
	{
		Errors.Clear();
		Tokens.Clear();

		InputFileFolderFullPath = fileInfo.DirectoryName ?? "";
		new FileLexer(this, fileInfo.Name, fileContent).Lex(fileInfo.FullName);

		if (Errors.Any())
		{
			throw new Exception("Lexical errors:\n- " + string.Join("\n- ", Errors));
		}

		return Tokens;
	}
}