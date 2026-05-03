using Runtime.Objects;

namespace UniversityP4.Tests;

public class MelodyTests
{
    [Fact]
    public void Melody_Should_Initialize_Empty()
    {
        var melody = new Melody();

        melody.LengthInBeats.ShouldBe(0);
        melody.Samples.ShouldBeEmpty();
        melody.Notes.ShouldBeEmpty();
    }

    [Fact]
    public void Melody_Should_Store_Notes_And_Samples()
    {
        var melody = new Melody { LengthInBeats = 8.0f };
        var note = new Note { Pitch = Pitch.FromString("C4"), StartBeat = 0.0f, EndBeat = 1.0f };
        var sample = new Sample { FilePath = "drums.wav" };
        melody.Notes.Add(note);
        melody.Samples.Add(sample);

        melody.Notes.Count.ShouldBe(1);
        melody.Notes[0].ShouldBe(note);
        melody.Samples.Count.ShouldBe(1);
        melody.Samples[0].FilePath.ShouldBe("drums.wav");
        melody.LengthInBeats.ShouldBe(8.0f);
    }
}

public class PatternTests
{
    [Fact]
    public void Pattern_Should_Initialize_Empty()
    {
        var pattern = new Pattern();

        pattern.LengthInBeats.ShouldBe(0);
        pattern.Patterns.ShouldBeEmpty();
        pattern.Melodies.ShouldBeEmpty();
    }

    [Fact]
    public void Pattern_Should_Store_Nested_Patterns_And_Melodies()
    {
        var pattern = new Pattern { LengthInBeats = 16.0f };
        var melody = new Melody { LengthInBeats = 4.0f };
        var nestedPattern = new Pattern { LengthInBeats = 4.0f };
        pattern.Melodies.Add(melody);
        pattern.Patterns.Add(nestedPattern);

        pattern.Melodies.Count.ShouldBe(1);
        pattern.Melodies[0].LengthInBeats.ShouldBe(4.0f);
        pattern.Patterns.Count.ShouldBe(1);
        pattern.Patterns[0].LengthInBeats.ShouldBe(4.0f);
    }
}

public class SampleTests
{
    [Fact]
    public void Sample_Should_Initialize_With_Defaults()
    {
        var sample = new Sample();

        sample.FilePath.ShouldBe("");
        sample.DelayBeats.ShouldBe(0.0f);
        sample.AttackBeats.ShouldBe(0.0f);
        sample.HoldBeats.ShouldBe(0.0f);
        sample.DecayBeats.ShouldBe(0.0f);
        sample.SustainLevel.ShouldBe(1.0f);
        sample.ReleaseBeats.ShouldBe(0.0f);
    }

    [Fact]
    public void Sample_Should_Store_Key_Properties()
    {
        var sample = new Sample
        {
            FilePath = "drums/kick.wav",
            ReferencePitch = Pitch.FromString("C3"),
            AttackBeats = 0.1f,
            HoldBeats = 0.2f,
            DecayBeats = 0.3f,
            SustainLevel = 0.8f,
            ReleaseBeats = 0.4f
        };

        sample.FilePath.ShouldBe("drums/kick.wav");
        sample.ReferencePitch.PitchClass.ShouldBe(0);
        sample.ReferencePitch.Octave.ShouldBe(3);
        sample.AttackBeats.ShouldBe(0.1f);
        sample.HoldBeats.ShouldBe(0.2f);
        sample.DecayBeats.ShouldBe(0.3f);
        sample.SustainLevel.ShouldBe(0.8f);
        sample.ReleaseBeats.ShouldBe(0.4f);
    }

    [Fact]
    public void Sample_Clone_Should_Create_Independent_Copy()
    {
        var original = new Sample
        {
            FilePath = "hi-hat.wav",
            ReferencePitch = Pitch.FromString("A4"),
            DelayBeats = 0.125f,
            AttackBeats = 0.05f,
            SustainLevel = 0.9f
        };

        var cloned = original.Clone();

        cloned.FilePath.ShouldBe(original.FilePath);
        cloned.DelayBeats.ShouldBe(original.DelayBeats);
        cloned.AttackBeats.ShouldBe(original.AttackBeats);
        cloned.SustainLevel.ShouldBe(original.SustainLevel);
    }

    [Fact]
    public void Sample_Clone_Should_Not_Share_References()
    {
        var original = new Sample
        {
            FilePath = "snare.wav",
            ReferencePitch = Pitch.FromString("G3")
        };

        var cloned = original.Clone();
        cloned.FilePath = "different.wav";

        original.FilePath.ShouldBe("snare.wav");
        cloned.FilePath.ShouldBe("different.wav");
    }
}
