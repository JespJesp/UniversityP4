using Runtime.Objects;

namespace UniversityP4.Tests;

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
    public void Note_Should_Store_StartBeat_And_EndBeat()
    {
        var note = new Note { StartBeat = 0.0f, EndBeat = 2.0f };

        note.StartBeat.ShouldBe(0.0f);
        note.EndBeat.ShouldBe(2.0f);
    }

    [Fact]
    public void Note_Should_Store_Pitch()
    {
        var pitch = Pitch.FromString("C4");
        var note = new Note { Pitch = pitch };

        note.Pitch.PitchClass.ShouldBe(pitch.PitchClass);
        note.Pitch.Octave.ShouldBe(pitch.Octave);
    }

    [Fact]
    public void Note_Should_Store_Volume()
    {
        var note = new Note { Volume = 0.8f };

        note.Volume.ShouldBe(0.8f);
    }

    [Fact]
    public void Note_Should_Store_Pan()
    {
        var note = new Note { Pan = -0.5f };

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
