using Phases.Annotation;
using Phases.Evaluation;
using Phases.Lexing;
using Phases.Parsing;
using Phases.Validation;

internal class Program
{
	static void Main(string[] args)
	{
		if (args.Length != 1)
		{
			throw new Exception("Program argument error: No file path provided to be interpreted");
		}

		string filePath = args[0];
		string fileContent = File.ReadAllText(filePath);
		string fileName = Path.GetFileName(filePath);
		string? fileFolderPath = Path.GetDirectoryName(filePath);

		if (fileFolderPath == null)
		{
			throw new Exception("Program argument error: Input file does not exist");
		}

		try
		{
			InterpretText(fileContent, fileName, fileFolderPath);
		}
		catch (Exception exception)
		{
			throw new Exception($"Interpretation error: {exception}");
		}
	}

	private static void InterpretText(string fileContent, string fileName, string fileFolderPath)
	{
		var tokens = new Lexer().Lex(fileContent, fileName, fileFolderPath);
		var astRoot = new Parser().Parse(tokens);
		new Annotator().Annotate(astRoot);
		new Validator().Validate(astRoot);
		new Evaluator().Evaluate(astRoot, fileFolderPath);
	}
}