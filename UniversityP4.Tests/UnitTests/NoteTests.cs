using Runtime.Objects;

namespace UniversityP4.Tests;

[Trait("Category","Unit")]
public class NoteTests
{
    [Fact]
    public void Note_Should_Initialize_Empty()
    {
        var note = new Note();

        note.Pitch.ShouldNotBeNull();
        note.Volume.ShouldBe(1.0f);
        note.Pan.ShouldBe(0.0f);
    }

    [Fact]
    public void Note_Should_Store_Key_Properties()
    {
        var note = new Note
        {
            StartBeat = 0.0f,
            EndBeat = 2.0f,
            Pitch = Pitch.FromString("C4"),
            Volume = 0.8f,
            Pan = -0.5f
        };

        note.StartBeat.ShouldBe(0.0f);
        note.EndBeat.ShouldBe(2.0f);
        note.Pitch.PitchClass.ShouldBe(0);
        note.Pitch.Octave.ShouldBe(4);
        note.Volume.ShouldBe(0.8f);
        note.Pan.ShouldBe(-0.5f);
    }

    [Fact]
    public void Note_Should_Support_Sample_Override()
    {
        var sample = new Sample { FilePath = "override.wav" };
        var note = new Note { SampleOverride = sample };

        note.SampleOverride.ShouldBe(sample);
        note.SampleOverride?.FilePath.ShouldBe("override.wav");
    }
}
