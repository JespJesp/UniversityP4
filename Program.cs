using LexicalAnalysis;
using SyntaxAnalysis;
using SemanticAnalysis;
using Evaluation;

internal class Program
{
	static void Main(string[] args)
	{
		string filePath = GetFilePath(args);
		string fileContent = GetFileContent(filePath);
		InterpretText(fileContent);
	}

	static string GetFilePath(string[] args)
	{
		if (args.Length == 0)
		{
			throw new Exception("Error getting file path from program argument: No file path provided.");
		}
		return args[0];
	}

	static string GetFileContent(string filePath)
	{
		try
		{
			return File.ReadAllText(filePath);
		}
		catch (FileNotFoundException)
		{
			throw new Exception($"Error reading file: File '{filePath}' not found.");
		}
		catch (Exception exception)
		{
			throw new Exception($"Error reading file: {exception.Message}");
		}
	}

	static void InterpretText(string text)
	{
		try
		{
			var tokens = new LexicalAnalyzer().Analyze(text);
			var pattern = new SyntaxAnalyzer().Analyze(tokens);
			new SemanticAnalyzer().Analyze(pattern);
			new Printer().Evaluate(pattern);
		}
		catch (Exception exception)
		{
			Console.WriteLine($"Error interpreting file: {exception.Message}");
		}
	}
}