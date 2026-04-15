using Ast;
using Ast.Nodes;
using Lexing;

internal class Program
{
	static void Main(string[] args)
	{
		if (args.Length != 1)
		{
			throw new Exception("Program argument error: No file path provided to be interpreted.");
		}

		string filePath = args[0];
		string fileContent = File.ReadAllText(filePath);
		string? fileFolderPath = Path.GetDirectoryName(filePath);

		if (fileFolderPath == null)
		{
			throw new Exception("Program argument error: Input file does not exist.");
		}

		try
		{
			InterpretText(fileContent, fileFolderPath);
		}
		catch (Exception exception)
		{
			throw new Exception($"Interpretation error: {exception}");
		}
	}

	private static void InterpretText(string fileText, string fileFolderPath)
	{
		var tokens = Lexer.Lex(fileText);
		var astRoot = Parser.Parse(tokens);
		Annotator.Annotate(astRoot);
		Validator.Validate(astRoot);
		Evaluator.Evaluate(astRoot, fileFolderPath);
	}
}