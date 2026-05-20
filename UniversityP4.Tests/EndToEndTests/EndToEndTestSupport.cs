using Ast.Nodes;
using Ast.Nodes.Timelines;
using Phases.Annotation;
using Phases.Evaluation;
using Phases.Lexing;
using Phases.Parsing;
using Phases.Validation;

namespace UniversityP4.Tests.EndToEndTests;

internal static class EndToEndTestSupport
{
	public static string RepoRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

	public static string TestProgramsRoot => Path.Combine(RepoRoot, "UniversityP4.Tests", "EndToEndTests", "TestPrograms");

	public static string AcceptanceTestsRoot => Path.Combine(TestProgramsRoot, "AcceptanceTests");

	public static string ProgramPath(string fileName) => Path.Combine(TestProgramsRoot, fileName);

	public static string AcceptanceProgramPath(string fileName) => Path.Combine(AcceptanceTestsRoot, fileName);

	public static FileInfo CreateOutputFile(string fileName)
	{
		return new FileInfo(Path.Combine(RepoRoot, fileName));
	}

	public static FileInfo CreateExampleProgramsOutputFile(string fileName)
	{
		return new FileInfo(Path.Combine(RepoRoot, "ExamplePrograms", fileName));
	}

	public static void DeleteIfExists(FileInfo fileInfo)
	{
		if (fileInfo.Exists)
		{
			fileInfo.Delete();
		}
	}

	public static void ResetGlobalState()
	{
		TimelineNode.InstanceCount = 0;
		SettingsNode.SettingsNodeInstances = 0;
	}

	public static FileNode ParseProgram(string filePath)
	{
		var fileInfo = new FileInfo(filePath);
		var content = File.ReadAllText(filePath);

		ResetGlobalState();

		var lexer = new Lexer();
		var tokens = lexer.Lex(content, fileInfo);

		var parser = new Parser();
		return parser.Parse(tokens);
	}

	public static FileNode RunPipelineToValidation(string filePath)
	{
		var program = ParseProgram(filePath);

		var annotator = new Annotator();
		annotator.Annotate(program);

		var validator = new Validator();
		validator.Validate(program);

		return program;
	}

	public static void RunFullPipeline(string filePath, FileInfo outputFile)
	{
		var program = RunPipelineToValidation(filePath);

		DeleteIfExists(outputFile);

		var evaluator = new Evaluator();
		evaluator.Evaluate(program, outputFile);
	}
}