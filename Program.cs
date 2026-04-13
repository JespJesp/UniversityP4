using Ast;
using Ast.Nodes;
using Lexing;

internal class Program
{
	static void Main(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("Error in program argument: No file path provided to be interpreted.");
			return;
		}

		string filePath = args[0];
		string fileContent = File.ReadAllText(filePath);
		string? fileFolderPath = Path.GetDirectoryName(filePath);

		if (fileFolderPath == null)
		{
			Console.WriteLine("Error in program argument: Input file does not exist.");
			return;
		}

		try
		{
			InterpretText(fileContent, fileFolderPath);
		}
		catch (Exception exception)
		{
			Console.WriteLine($"Error interpreting file: {exception}");
		}
	}

	private static void InterpretText(string fileText, string fileFolderPath)
	{
		var tokens = Lexer.Lex(fileText);
		ProgramNode astRoot = Parser.ParseTree(tokens);
		Annotator.ValidateTree(astRoot);
		Evaluator.EvaluateTree(astRoot, fileFolderPath);
	}
}