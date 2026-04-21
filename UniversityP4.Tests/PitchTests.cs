using Runtime.Objects;

namespace UniversityP4.Tests;

public class PitchTests
{
    [Fact]
    public void Constructor_Should_Parse_Natural_Note_And_Octave()
    {
        var pitch = new Pitch("C5");

        pitch.PitchClass.ShouldBe(0);
        pitch.Octave.ShouldBe(5);
    }

    [Fact]
    public void Constructor_Should_Treat_Enharmonic_Notes_As_Same_PitchClass()
    {
        var cSharp = new Pitch("C#3");
        var dFlat = new Pitch("Db3");

        cSharp.PitchClass.ShouldBe(1);
        dFlat.PitchClass.ShouldBe(1);
        cSharp.Octave.ShouldBe(3);
        dFlat.Octave.ShouldBe(3);
    }

    [Fact]
    public void Constructor_Should_Be_Case_Insensitive()
    {
        var upper = new Pitch("G#4");
        var lower = new Pitch("g#4");

        lower.PitchClass.ShouldBe(upper.PitchClass);
        lower.Octave.ShouldBe(upper.Octave);
    }

    [Fact]
    public void Constructor_Should_Throw_For_Unknown_PitchClass()
    {
        Action act = () => _ = new Pitch("H5");

        var exception = Should.Throw<Exception>(act);

        exception.Message.ShouldContain("Pitch class 'h' is not recognized.");
    }

    [Fact]
    public void Constructor_Should_Throw_For_NonUnsigned_Octave()
    {
        Action act = () => _ = new Pitch("C9999999999999999999999999999");

        var exception = Should.Throw<Exception>(act);

        exception.Message.ShouldContain("Octave");
        exception.Message.ShouldContain("not an unsigned integer");
    }
}
