using System.Collections.Generic;
using Phases.Evaluation;

namespace UniversityP4.Tests.EndToEndTests;

[Collection("EndToEnd")]
[Trait("Category","EndToEnd")]
public class EvaluatorErrorTests
{
    public static IEnumerable<object[]> ProgramsThatFailAtParsePhase()
    {
        yield return new object[] { EndToEndTestSupport.ProgramPath("invalid_syntax_missing_float.mude"), "Missing float value" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("expressions_invalid.mude"), "Invalid arithmetic expression (parse error)" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("chords_invalid.mude"), "Malformed chord syntax (missing parenthesis)" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("heavy_program_invalid.mude"), "Unexpected token in heavy program (parse error)" };
    }

    public static IEnumerable<object[]> ProgramsThatFailAtValidationPhase()
    {
        yield return new object[] { EndToEndTestSupport.ProgramPath("invalid_duplicate_timeline.mude"), "Duplicate timeline declaration" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("invalid_undeclared_melody.mude"), "Undeclared melody reference" };
        yield return new object[] { EndToEndTestSupport.AcceptanceProgramPath("InvalidPanningError.mude"), "Invalid panning value" };
        yield return new object[] { EndToEndTestSupport.AcceptanceProgramPath("InvalidSampleError.mude"), "Nonexistent sample file" };
    }

    public static IEnumerable<object[]> ProgramsThatFailAtEvaluationPhase()
    {
        yield return new object[] { EndToEndTestSupport.ProgramPath("exceeds_one_hour.mude"), "Audio duration exceeds 1 hour limit" };
    }

    public static IEnumerable<object[]> ProgramsThatFailAtLexingPhase()
    {
        yield return new object[] { EndToEndTestSupport.ProgramPath("invalid_lexing_error.mude"), "Unterminated string (lexing error)" };
    }

    public static IEnumerable<object[]> ProgramsThatFailAtAnnotationPhase()
    {
        yield return new object[] { EndToEndTestSupport.ProgramPath("invalid_annotation_error.mude"), "Reference to sample before declaration (annotation error)" };
    }

    [Theory]
    [MemberData(nameof(ProgramsThatFailAtParsePhase))]
    public void Program_With_Parse_Error_Should_Fail(string filePath, string errorDescription)
    {
        var outputFile = EndToEndTestSupport.CreateOutputFile("e2e_parse_error.wav");

        try
        {
            Should.Throw<Exception>(() => EndToEndTestSupport.RunFullPipeline(filePath, outputFile));
        }
        finally
        {
            EndToEndTestSupport.DeleteIfExists(outputFile);
        }
    }

    [Theory]
    [MemberData(nameof(ProgramsThatFailAtValidationPhase))]
    public void Program_With_Validation_Error_Should_Fail(string filePath, string errorDescription)
    {
        var outputFile = EndToEndTestSupport.CreateOutputFile("e2e_validation_error.wav");

        try
        {
            Should.Throw<Exception>(() => EndToEndTestSupport.RunFullPipeline(filePath, outputFile));
        }
        finally
        {
            EndToEndTestSupport.DeleteIfExists(outputFile);
        }
    }

    [Theory]
    [MemberData(nameof(ProgramsThatFailAtEvaluationPhase))]
    public void Program_With_Evaluation_Error_Should_Fail(string filePath, string errorDescription)
    {
        var outputFile = EndToEndTestSupport.CreateOutputFile("e2e_evaluation_error.wav");

        try
        {
            Should.Throw<Exception>(() => EndToEndTestSupport.RunFullPipeline(filePath, outputFile));
        }
        finally
        {
            EndToEndTestSupport.DeleteIfExists(outputFile);
        }
    }

    [Theory]
    [MemberData(nameof(ProgramsThatFailAtLexingPhase))]
    public void Program_With_Lexing_Error_Should_Fail(string filePath, string errorDescription)
    {
        var outputFile = EndToEndTestSupport.CreateOutputFile("e2e_lexing_error.wav");

        try
        {
            Should.Throw<Exception>(() => EndToEndTestSupport.RunFullPipeline(filePath, outputFile));
        }
        finally
        {
            EndToEndTestSupport.DeleteIfExists(outputFile);
        }
    }

    [Theory]
    [MemberData(nameof(ProgramsThatFailAtAnnotationPhase))]
    public void Program_With_Annotation_Error_Should_Fail(string filePath, string errorDescription)
    {
        var outputFile = EndToEndTestSupport.CreateOutputFile("e2e_annotation_error.wav");

        try
        {
            Should.Throw<Exception>(() => EndToEndTestSupport.RunFullPipeline(filePath, outputFile));
        }
        finally
        {
            EndToEndTestSupport.DeleteIfExists(outputFile);
        }
    }
}

