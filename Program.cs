using LexicalAnalysis;
using SyntaxAnalysis;
using SemanticAnalysis;
using Evaluation;

internal class Program
{
	static void Main(string[] args)
	{
		if (args.Length == 0)
		{
			throw new Exception("Error in program argument: No file path provided to be interpreted.");
		}

		string filePath = args[0];
		string fileText = File.ReadAllText(filePath);
		InterpretText(fileText);
	}

	private static void InterpretText(string text)
	{
		try
		{
			var tokens = new LexicalAnalyzer().Lex(text);
			var song = new SyntaxAnalyzer().Parse(tokens);
			new SemanticAnalyzer().Validate(song);
			new Evaluator().Evaluate(song);
		}
		catch (Exception exception)
		{
			Console.WriteLine($"Error interpreting file: {exception.Message}");
		}
	}
}