using System.Collections.Generic;
using Ast;
using Ast.Nodes;
using Ast.Nodes.Timelines;
using Phases.Annotation;
using Phases.Evaluation;
using Phases.Lexing;
using Phases.Parsing;
using Phases.Validation;

namespace UniversityP4.Tests.IntegrationTests;

public class IntegrationTestFixture
{
    protected static string RepoRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

    protected static FileNode CreateProgramNode(out TimelineNode timelineNode)
    {
        var programNode = new FileNode { Location = new Location("file.mude", 1, 1) };
        timelineNode = new TimelineNode();
        TimelineNode.Instance = timelineNode;
        TimelineNode.InstanceCount = 1;

        var melodyNode = new Ast.Nodes.Melodies.MelodyNode
        {
            Id = "_lead",
            Melody = new Runtime.Objects.Melody
            {
                LengthInBeats = 1f,
                Notes =
                {
                    new Runtime.Objects.Note
                    {
                        StartBeat = 0f,
                        EndBeat = 1f,
                        Pitch = Runtime.Objects.Pitch.FromString("C4")
                    }
                },
                Samples =
                {
                    new Runtime.Objects.Sample
                    {
                        FilePath = "/ExamplePrograms/Samples/Drums/snare.wav"
                    }
                }
            }
        };

        timelineNode.SymbolTable.Upsert(melodyNode, melodyNode.Id);
        timelineNode.Timeline.Commands.Add(new Runtime.Objects.Timelines.TimelineCommand
        {
            Type = Runtime.Objects.Timelines.TimelineCommandType.Start,
            Beat = 0,
            TargetIds = new List<string> { "_lead" }
        });
        timelineNode.Timeline.Commands.Add(new Runtime.Objects.Timelines.TimelineCommand
        {
            Type = Runtime.Objects.Timelines.TimelineCommandType.Stop,
            Beat = 1,
            TargetIds = new List<string> { "_lead" }
        });

        programNode.Children.Add(timelineNode);

        return programNode;
    }

    protected static FileInfo CreateFileInfo(string fileName = "UniversityP4.IntegrationTests.wav")
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current != null && !Directory.Exists(Path.Combine(current.FullName, "ExamplePrograms")))
        {
            current = current.Parent;
        }

        var projectRoot = current?.FullName ?? Directory.GetCurrentDirectory();
        return new FileInfo(Path.Combine(projectRoot, fileName));
    }

    protected static void DeleteIfExists(FileInfo fileInfo)
    {
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
    }

    protected static FileNode ParseProgram(string source, string fileName = "UniversityP4.IntegrationTests.mude")
    {
        ResetGlobalState();

        var fileInfo = new FileInfo(Path.Combine(RepoRoot, fileName));
        var lexer = new Lexer();
        var tokens = lexer.Lex(source, fileInfo);

        var parser = new Parser();
        return parser.Parse(tokens);
    }

    protected static FileNode RunPipelineToValidation(string source, string fileName = "UniversityP4.IntegrationTests.mude")
    {
        var program = ParseProgram(source, fileName);

        var annotator = new Annotator();
        annotator.Annotate(program);

        var validator = new Validator();
        validator.Validate(program);

        return program;
    }

    protected static void RunFullPipeline(string source, FileInfo outputFile, string fileName = "UniversityP4.IntegrationTests.mude")
    {
        var program = RunPipelineToValidation(source, fileName);

        if (outputFile.Exists)
        {
            outputFile.Delete();
        }

        var evaluator = new Evaluator();
        evaluator.Evaluate(program, outputFile);
    }

    protected static void ResetGlobalState()
    {
        TimelineNode.InstanceCount = 0;
        Ast.Nodes.Timelines.SettingsNode.SettingsNodeInstances = 0;
    }

    protected class TrackingEvaluationNode : Node
    {
        public bool EvaluateWasCalled { get; private set; }

        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Evaluate(Evaluator evaluator)
        {
            EvaluateWasCalled = true;
        }
    }

    protected class OrderTrackingEvaluationNode : Node
    {
        public List<string>? CallOrder { get; set; }
        public string? NodeName { get; set; }

        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Evaluate(Evaluator evaluator)
        {
            if (CallOrder != null && NodeName != null)
            {
                CallOrder.Add(NodeName);
            }
        }
    }

    protected class ErrorThrowingEvaluationNode : Node
    {
        public override void CascadeParse(Phases.Parsing.Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Evaluate(Evaluator evaluator)
        {
            throw new Exception("Evaluation failed");
        }
    }
}
