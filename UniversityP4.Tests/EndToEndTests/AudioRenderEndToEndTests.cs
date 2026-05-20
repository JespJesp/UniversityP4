using System.Collections.Generic;
using Phases.Evaluation;

namespace UniversityP4.Tests.EndToEndTests;

[Collection("EndToEnd")]
[Trait("Category","EndToEnd")]
public class AudioRenderEndToEndTests
{
    public static IEnumerable<object[]> ValidProgramsWithDifferentFeatures()
    {
        yield return new object[] { EndToEndTestSupport.ProgramPath("minimal_program.mude"), "Basic melody and sample" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("samples_only.mude"), "Sample declarations" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("notes_list.mude"), "Multiple notes in chords" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("expressions_gain.mude"), "Arithmetic expressions in attributes" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("chords_multiple.mude"), "Multiple chord lines" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("panning_stereo.mude"), "Panning and stereo effects" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("heavy_program.mude"), "Complex multi-note melody" };
        yield return new object[] { EndToEndTestSupport.ProgramPath("multiple_timelines.mude"), "Multiple timeline sections" };
        yield return new object[] { EndToEndTestSupport.AcceptanceProgramPath("MelodyBassPatternValid.mude"), "Complex orchestration" };
    }

    [Theory]
    [MemberData(nameof(ValidProgramsWithDifferentFeatures))]
    public void ValidProgram_Should_Successfully_Complete_Full_Pipeline(string filePath, string feature)
    {
        var repoRoot = EndToEndTestSupport.RepoRoot;
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
        var testOutputFile = new FileInfo(Path.Combine(repoRoot, $"e2e_test_{fileNameWithoutExt}.wav"));

        try
        {
            EndToEndTestSupport.DeleteIfExists(testOutputFile);

            EndToEndTestSupport.RunFullPipeline(filePath, testOutputFile);

            testOutputFile.Refresh();
            testOutputFile.Exists.ShouldBeTrue($"{feature}: Audio file should be created");
            testOutputFile.Length.ShouldBeGreaterThan(0, $"{feature}: Audio file should not be empty");
        }
        finally
        {
            EndToEndTestSupport.DeleteIfExists(testOutputFile);
        }
    }
}
