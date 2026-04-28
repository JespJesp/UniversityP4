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
    public void Melody_Should_Store_LengthInBeats()
    {
        var melody = new Melody { LengthInBeats = 4.0f };

        melody.LengthInBeats.ShouldBe(4.0f);
    }

    [Fact]
    public void Melody_Should_Add_Notes()
    {
        var melody = new Melody();
        var note = new Note { Pitch = Pitch.FromString("C4"), StartBeat = 0.0f, EndBeat = 1.0f };
        melody.Notes.Add(note);

        melody.Notes.Count.ShouldBe(1);
        melody.Notes[0].ShouldBe(note);
    }

    [Fact]
    public void Melody_Should_Add_Multiple_Notes()
    {
        var melody = new Melody();
        var note1 = new Note { Pitch = Pitch.FromString("C4"), StartBeat = 0.0f, EndBeat = 1.0f };
        var note2 = new Note { Pitch = Pitch.FromString("D4"), StartBeat = 1.0f, EndBeat = 2.0f };
        var note3 = new Note { Pitch = Pitch.FromString("E4"), StartBeat = 2.0f, EndBeat = 3.0f };
        
        melody.Notes.Add(note1);
        melody.Notes.Add(note2);
        melody.Notes.Add(note3);

        melody.Notes.Count.ShouldBe(3);
    }

    [Fact]
    public void Melody_Should_Add_Samples()
    {
        var melody = new Melody();
        var sample = new Sample { FilePath = "drums.wav" };
        melody.Samples.Add(sample);

        melody.Samples.Count.ShouldBe(1);
        melody.Samples[0].FilePath.ShouldBe("drums.wav");
    }

    [Fact]
    public void Melody_Should_Support_Mixed_Notes_And_Samples()
    {
        var melody = new Melody { LengthInBeats = 8.0f };
        melody.Notes.Add(new Note { Pitch = Pitch.FromString("C4"), StartBeat = 0.0f, EndBeat = 2.0f });
        melody.Notes.Add(new Note { Pitch = Pitch.FromString("G4"), StartBeat = 2.0f, EndBeat = 4.0f });
        melody.Samples.Add(new Sample { FilePath = "kick.wav" });

        melody.Notes.Count.ShouldBe(2);
        melody.Samples.Count.ShouldBe(1);
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
    public void Pattern_Should_Store_LengthInBeats()
    {
        var pattern = new Pattern { LengthInBeats = 16.0f };

        pattern.LengthInBeats.ShouldBe(16.0f);
    }

    [Fact]
    public void Pattern_Should_Add_Melodies()
    {
        var pattern = new Pattern();
        var melody = new Melody { LengthInBeats = 4.0f };
        pattern.Melodies.Add(melody);

        pattern.Melodies.Count.ShouldBe(1);
        pattern.Melodies[0].LengthInBeats.ShouldBe(4.0f);
    }

    [Fact]
    public void Pattern_Should_Add_Multiple_Melodies()
    {
        var pattern = new Pattern();
        pattern.Melodies.Add(new Melody { LengthInBeats = 4.0f });
        pattern.Melodies.Add(new Melody { LengthInBeats = 4.0f });
        pattern.Melodies.Add(new Melody { LengthInBeats = 4.0f });

        pattern.Melodies.Count.ShouldBe(3);
    }

    [Fact]
    public void Pattern_Should_Add_Nested_Patterns()
    {
        var pattern = new Pattern { LengthInBeats = 16.0f };
        var nestedPattern = new Pattern { LengthInBeats = 4.0f };
        pattern.Patterns.Add(nestedPattern);

        pattern.Patterns.Count.ShouldBe(1);
        pattern.Patterns[0].LengthInBeats.ShouldBe(4.0f);
    }

    [Fact]
    public void Pattern_Should_Support_Multiple_Nesting_Levels()
    {
        var rootPattern = new Pattern { LengthInBeats = 32.0f };
        var level1Pattern = new Pattern { LengthInBeats = 16.0f };
        var level2Pattern = new Pattern { LengthInBeats = 8.0f };
        var melody = new Melody { LengthInBeats = 4.0f };

        level2Pattern.Melodies.Add(melody);
        level1Pattern.Patterns.Add(level2Pattern);
        rootPattern.Patterns.Add(level1Pattern);

        rootPattern.Patterns.Count.ShouldBe(1);
        rootPattern.Patterns[0].Patterns.Count.ShouldBe(1);
        rootPattern.Patterns[0].Patterns[0].Melodies.Count.ShouldBe(1);
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
    public void Sample_Should_Store_FilePath()
    {
        var sample = new Sample { FilePath = "drums/kick.wav" };

        sample.FilePath.ShouldBe("drums/kick.wav");
    }

    [Fact]
    public void Sample_Should_Store_ReferencePitch()
    {
        var pitch = Pitch.FromString("C3");
        var sample = new Sample { ReferencePitch = pitch };

        sample.ReferencePitch.PitchClass.ShouldBe(pitch.PitchClass);
        sample.ReferencePitch.Octave.ShouldBe(pitch.Octave);
    }

    [Fact]
    public void Sample_Should_Store_ADSR_Parameters()
    {
        var sample = new Sample
        {
            AttackBeats = 0.1f,
            HoldBeats = 0.2f,
            DecayBeats = 0.3f,
            SustainLevel = 0.8f,
            ReleaseBeats = 0.4f
        };

        sample.AttackBeats.ShouldBe(0.1f);
        sample.HoldBeats.ShouldBe(0.2f);
        sample.DecayBeats.ShouldBe(0.3f);
        sample.SustainLevel.ShouldBe(0.8f);
        sample.ReleaseBeats.ShouldBe(0.4f);
    }

    [Fact]
    public void Sample_Should_Store_DelayBeats()
    {
        var sample = new Sample { DelayBeats = 0.5f };

        sample.DelayBeats.ShouldBe(0.5f);
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
