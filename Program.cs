using Ast;
using Ast.Nodes;
using Lexing;

internal class Program
{
	static void Main(string[] args)
	{
		if (args.Length != 1)
		{
			throw new Exception("Error in program argument: No file path provided to be interpreted.");
		}

		string filePath = args[0];
		string fileText = File.ReadAllText(filePath);

		try
		{
			InterpretText(fileText);
		}
		catch (Exception exception)
		{
			Console.WriteLine($"Error interpreting file: {exception}");
		}
	}

	private static void InterpretText(string text)
	{
		var tokens = Lexer.Lex(text);
		ProgramNode astRoot = Parser.ParseTree(tokens);
		Annotator.AnnotateTree(astRoot);
		Evaluator.EvaluateTree(astRoot);
	}
}