using Phases.Evaluation;

namespace UniversityP4.Tests.IntegrationTests;

[Collection("Integration")]
[Trait("Category","Integration")]
public class InMemoryProgramIntegrationTests : IntegrationTestFixture
{
    [Fact]
    public void FullPipeline_Should_Complete_For_Minimal_InMemory_Program()
    {
        ResetGlobalState();

        var source = string.Join("\n", new[]
        {
            "sample guitar \"/Samples/Instruments/electric guitar E4.wav\" e4",
            "",
            "float exampleNumber 1.0",
            "",
            "melody 1 _test",
            "\tsamples",
            "\t\tguitar",
            "\tchords",
            "\t\t0,1 c4",
            "",
            "timeline",
            "\tsettings",
            "\t\tbpm 120",
            "\t\ttimesignature 4,4",
            "\tstart 0",
            "\t\t1_test",
            "\tstop 1",
            "\t\t1_test",
        });

        var outputFile = CreateFileInfo("InMemoryMinimalProgram.wav");

        try
        {
            DeleteIfExists(outputFile);

            RunFullPipeline(source, outputFile);

            outputFile.Refresh();
            outputFile.Exists.ShouldBeTrue();
            outputFile.Length.ShouldBeGreaterThan(0);
        }
        finally
        {
            DeleteIfExists(outputFile);
        }
    }

    [Fact]
    public void FullPipeline_Should_Complete_For_InMemory_Program_With_String_Expression()
    {
        ResetGlobalState();

        var source = string.Join("\n", new[]
        {
            "string instrumentsFolder \"/Samples/Instruments\"",
            "",
            "sample guitar instrumentsFolder+\"/electric guitar E4.wav\" e4",
            "sample flute instrumentsFolder+\"/flute E.wav\" c5",
            "",
            "melody 8 _sample_test",
            "\tsamples",
            "\t\tguitar",
            "\tchords",
            "\t\t0,2 c4",
            "\t\t2,4 e4 g4",
            "",
            "timeline",
            "\tsettings",
            "\t\tbpm 110",
            "\t\ttimesignature 4,4",
            "\tstart 0",
            "\t\t8_sample_test",
            "\tstop 4",
            "\t\t8_sample_test",
        });

        var outputFile = CreateFileInfo("InMemoryExpressionProgram.wav");

        try
        {
            DeleteIfExists(outputFile);

            RunFullPipeline(source, outputFile);

            outputFile.Refresh();
            outputFile.Exists.ShouldBeTrue();
            outputFile.Length.ShouldBeGreaterThan(0);
        }
        finally
        {
            DeleteIfExists(outputFile);
        }
    }

    [Fact]
    public void RunPipelineToValidation_Should_Report_Validation_Error_For_Duplicate_Timeline()
    {
        ResetGlobalState();

        var source = string.Join("\n", new[]
        {
            "sample guitar \"/Samples/Instruments/electric guitar E4.wav\" e4",
            "",
            "melody 1 _test",
            "\tsamples",
            "\t\tguitar",
            "\tchords",
            "\t\t0,1 c4",
            "",
            "timeline",
            "\tsettings",
            "\t\tbpm 120",
            "\t\ttimesignature 4,4",
            "\tstart 0",
            "\t\t1_test",
            "\tstop 1",
            "\t\t1_test",
            "",
            "timeline",
            "\tstart 0",
            "\t\t1_test",
            "\tstop 1",
            "\t\t1_test",
        });

        var exception = Should.Throw<Exception>(() => RunPipelineToValidation(source));

        exception.Message.ShouldContain("Semantic errors (from annotation phase)");
        exception.Message.ShouldContain("Line: '18'");
    }

    [Fact]
    public void Validation_Should_Fail_When_Bpm_Is_String()
    {
        ResetGlobalState();

        var source = string.Join("\n", new[]
        {
            "string badVal \"oops\"",
            "sample guitar \"/Samples/Instruments/electric guitar E4.wav\" e4",
            "melody 1 _test",
            "\tsamples",
            "\t\tguitar",
            "\tchords",
            "\t\t0,1 c4",
            "\n",
            "timeline",
            "\tsettings",
            "\t\tbpm badVal",
            "\t\ttimesignature 4,4",
            "\tstart 0",
            "\t\t1_test",
            "\tstop 1",
            "\t\t1_test",
        });

        var ex = Should.Throw<Exception>(() => RunPipelineToValidation(source));
        ex.Message.ShouldContain("Semantic errors");
    }

    [Fact]
    public void Validation_Should_Fail_When_Gain_Is_String()
    {
        ResetGlobalState();

        var source = string.Join("\n", new[]
        {
            "string s \"bad\"",
            "sample bass \"/Samples/Instruments/bass G.wav\" c2",
            "melody 1 _test",
            "\tsamples",
            "\t\tbass",
            "\tchords",
            "\t\t0,1 c2(gain s)",
            "\n",
            "timeline",
            "\tsettings",
            "\t\tbpm 90",
            "\t\ttimesignature 4,4",
            "\tstart 0",
            "\t\t1_test",
            "\tstop 1",
            "\t\t1_test",
        });

        var ex = Should.Throw<Exception>(() => RunPipelineToValidation(source));
        ex.Message.ShouldContain("Semantic errors");
    }

    [Fact]
    public void Validation_Should_Fail_For_Duplicate_Melody_Id()
    {
        ResetGlobalState();

        var source = string.Join("\n", new[]
        {
            "sample bass \"/Samples/Instruments/bass G.wav\" c2",
            "melody 1 _dup",
            "\tsamples",
            "\t\tbass",
            "\tchords",
            "\t\t0,1 c2",
            "melody 1 _dup",
            "\tsamples",
            "\t\tbass",
            "\tchords",
            "\t\t0,1 c2",
            "\n",
            "timeline",
            "\tsettings",
            "\t\tbpm 90",
            "\t\ttimesignature 4,4",
            "\tstart 0",
            "\t\t1_dup",
            "\tstop 1",
            "\t\t1_dup",
        });

        var ex = Should.Throw<Exception>(() => RunPipelineToValidation(source));
        ex.Message.ShouldContain("Semantic errors");
    }

    [Fact]
    public void Validation_Should_Fail_For_Duplicate_Float_Declaration()
    {
        ResetGlobalState();

        var source = string.Join("\n", new[]
        {
            "float a 1.0",
            "float a 2.0",
            "sample guitar \"/Samples/Instruments/electric guitar E4.wav\" e4",
            "melody 1 _test",
            "\tsamples",
            "\t\tguitar",
            "\tchords",
            "\t\t0,1 c4",
            "\n",
            "timeline",
            "\tsettings",
            "\t\tbpm 120",
            "\t\ttimesignature 4,4",
            "\tstart 0",
            "\t\t1_test",
            "\tstop 1",
            "\t\t1_test",
        });

        var ex = Should.Throw<Exception>(() => RunPipelineToValidation(source));
        ex.Message.ShouldContain("Semantic errors");
    }
}