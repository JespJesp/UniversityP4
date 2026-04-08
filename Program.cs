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
		Console.WriteLine("Parsing..."); // TODO: REMOVE; FOR DEBUGGING
		ProgramNode astRoot = Parser.ParseTree(tokens);
		Console.WriteLine("Annotating..."); // TODO: REMOVE; FOR DEBUGGING
		Annotator.AnnotateTree(astRoot);
		Console.WriteLine("Evaluating"); // TODO: REMOVE; FOR DEBUGGING
		Evaluator.EvaluateTree(astRoot);
	}
}