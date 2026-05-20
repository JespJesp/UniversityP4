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
		FileInfo fileInfo = new FileInfo(filePath);

		if (!fileInfo.Exists)
		{
			throw new Exception("Program argument error: Input file does not exist");
		}

		string fileContent = File.ReadAllText(fileInfo.FullName);

		try
		{
			InterpretFile(fileContent, fileInfo);
		}
		catch (Exception exception)
		{
			throw new Exception($"Interpretation error: {exception}");
		}
	}

	private static void InterpretFile(string fileContent, FileInfo fileInfo)
	{
		var tokens = new Lexer().Lex(fileContent, fileInfo);
		var astRoot = new Parser().Parse(tokens);
		new Annotator().Annotate(astRoot);
		new Validator().Validate(astRoot);
		
		// Create output file in the same directory as input file
		var outputPath = Path.Combine(fileInfo.DirectoryName ?? "", "ProgramOutput.wav");
		var outputFile = new FileInfo(outputPath);
		
		new Evaluator().Evaluate(astRoot, outputFile);
	}
}