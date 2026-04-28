using Ast;
using Ast.Nodes.Melodies;
using Runtime.AudioRendering.Loops;
using Runtime.Objects;
using Runtime.Objects.Timelines;

namespace UniversityP4.Tests;

public class TimelineLoopBuilderTests
{
    [Fact]
    public void Build_Should_Create_Loop_From_Start_And_Stop_With_Adjusted_Melody()
    {
        var melody = new Melody
        {
            LengthInBeats = 2f,
            Notes =
            {
                new Note
                {
                    StartBeat = 0,
                    EndBeat = 1,
                    Pitch = Pitch.FromString("C4"),
                    Volume = 0.8f
                }
            }
        };

        var symbols = CreateSymbols((typeof(MelodyNode), "_lead", CreateMelodyNode("_lead", melody)));

        var timeline = new Timeline();
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Start,
            Beat = 0,
            TargetIds = new List<string> { "_lead" },
            GainMultiplier = 0.5f,
            PitchShiftHalfsteps = 12f
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Stop,
            Beat = 4,
            TargetIds = new List<string> { "_lead" }
        });

        var loops = new LoopBuilder().Build(timeline, symbols);

        loops.Count.ShouldBe(1);
        var createdLoop = loops[0];
        createdLoop.StartBeat.ShouldBe(0f);
        createdLoop.EndBeat.ShouldBe(4f);

        createdLoop.Melody.ShouldNotBeSameAs(melody);
        createdLoop.Melody.Notes.Count.ShouldBe(1);
        createdLoop.Melody.Notes[0].Volume.ShouldBe(0.4f, 0.0001f);
        createdLoop.Melody.Notes[0].Pitch.Octave.ShouldBe(5);
        createdLoop.Melody.Notes[0].Pitch.PitchClass.ShouldBe(0);
    }

    [Fact]
    public void Build_Should_Close_Open_Loops_At_End_Of_Bar()
    {
        var melody = new Melody
        {
            LengthInBeats = 2f,
            Notes =
            {
                new Note
                {
                    StartBeat = 0,
                    EndBeat = 1,
                    Pitch = Pitch.FromString("C4")
                }
            }
        };

        var symbols = CreateSymbols((typeof(MelodyNode), "_lead", CreateMelodyNode("_lead", melody)));

        var timeline = new Timeline
        {
            BeatsPerBar = 4
        };
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Start,
            Beat = 1,
            TargetIds = new List<string> { "_lead" }
        });

        var loops = new LoopBuilder().Build(timeline, symbols);

        loops.Count.ShouldBe(1);
        loops[0].StartBeat.ShouldBe(1f);
        loops[0].EndBeat.ShouldBe(4f);
    }

    [Fact]
    public void Build_Should_Ignore_Stop_Without_Targets_And_Id()
    {
        var melody = new Melody { LengthInBeats = 1f };
        var symbols = CreateSymbols((typeof(MelodyNode), "_lead", CreateMelodyNode("_lead", melody)));

        var timeline = new Timeline();
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Start,
            Beat = 0,
            TargetIds = new List<string> { "_lead" }
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Stop,
            Beat = 4,
            TargetIds = new List<string> { "_lead" }
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Stop,
            Beat = 8
        });

        var loops = new LoopBuilder().Build(timeline, symbols);

        loops.Count.ShouldBe(1);
        loops[0].StartBeat.ShouldBe(0f);
        loops[0].EndBeat.ShouldBe(4f);
    }

    [Fact]
    public void Build_Should_Stop_By_Command_Id_When_No_Targets_Are_Provided()
    {
        var melody = new Melody { LengthInBeats = 1f };
        var symbols = CreateSymbols((typeof(MelodyNode), "_lead", CreateMelodyNode("_lead", melody)));

        var timeline = new Timeline();
        timeline.Commands.Add(new TimelineCommand
        {
            Id = "verse",
            Type = TimelineCommandType.Start,
            Beat = 4,
            TargetIds = new List<string> { "_lead" }
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Id = "verse",
            Type = TimelineCommandType.Stop,
            Beat = 8
        });

        var loops = new LoopBuilder().Build(timeline, symbols);

        loops.Count.ShouldBe(1);
        loops[0].StartBeat.ShouldBe(4f);
        loops[0].EndBeat.ShouldBe(8f);
    }

    [Fact]
    public void Build_Should_Stop_Everything_Case_Insensitively()
    {
        var lead = new Melody { LengthInBeats = 1f };
        var bass = new Melody { LengthInBeats = 1f };

        var symbols = CreateSymbols(
            (typeof(MelodyNode), "_lead", CreateMelodyNode("_lead", lead)),
            (typeof(MelodyNode), "_bass", CreateMelodyNode("_bass", bass)));

        var timeline = new Timeline();
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Start,
            Beat = 0,
            TargetIds = new List<string> { "_lead", "_bass" }
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Stop,
            Beat = 4,
            TargetIds = new List<string> { "everything" }
        });

        var loops = new LoopBuilder().Build(timeline, symbols);

        loops.Count.ShouldBe(2);
        loops.All(loop => loop.EndBeat == 4f).ShouldBeTrue();
    }

    private static MelodyNode CreateMelodyNode(string id, Melody melody)
    {
        return new MelodyNode
        {
            Id = id,
            Melody = melody
        };
    }

    private static SymbolTable CreateSymbols(params (Type type, string id, SymbolNode node)[] entries)
    {
        var table = new SymbolTable();

        foreach (var (type, id, node) in entries)
        {
            table.Symbols[(type, id)] = node;
        }

        return table;
    }
}
