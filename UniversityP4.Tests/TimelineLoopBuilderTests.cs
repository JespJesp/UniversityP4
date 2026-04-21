using System.Reflection;
using Ast.Tables;
using Runtime.Objects;

namespace UniversityP4.Tests;

public class TimelineLoopBuilderTests
{
    [Fact]
    public void BuildLoopsFromCommands_Should_Create_Loop_From_Start_And_Stop_With_Adjusted_Melody()
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
                    Pitch0 = new Pitch("C4"),
                    Volume = 0.8f
                }
            }
        };

        var variables = CreateVariables(((typeof(Melody), "_lead"), melody));

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

        timeline.BuildLoopsFromCommands(variables);

        timeline.Loops.Count.ShouldBe(1);
        var createdLoop = timeline.Loops[0];
        createdLoop.StartBeat.ShouldBe(0f);
        createdLoop.EndBeat.ShouldBe(4f);

        createdLoop.Melody0.ShouldNotBeSameAs(melody);
        createdLoop.Melody0.Notes.Count.ShouldBe(1);
        createdLoop.Melody0.Notes[0].Volume.ShouldBe(0.4f, 0.0001f);
        createdLoop.Melody0.Notes[0].Pitch0.Octave.ShouldBe(5);
        createdLoop.Melody0.Notes[0].Pitch0.PitchClass.ShouldBe(0);
    }

    [Fact]
    public void BuildLoopsFromCommands_Should_Close_Open_Loops_At_End_Of_Bar()
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
                    Pitch0 = new Pitch("C4")
                }
            }
        };

        var variables = CreateVariables(((typeof(Melody), "_lead"), melody));

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

        timeline.BuildLoopsFromCommands(variables);

        timeline.Loops.Count.ShouldBe(1);
        timeline.Loops[0].StartBeat.ShouldBe(1f);
        timeline.Loops[0].EndBeat.ShouldBe(4f);
    }

    [Fact]
    public void BuildLoopsFromCommands_Should_Use_Previous_Beat_When_Stop_Command_Has_No_Beat()
    {
        var melody = new Melody { LengthInBeats = 1f };
        var variables = CreateVariables(((typeof(Melody), "_lead"), melody));

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
            TargetIds = new List<string> { "_lead" }
        });

        timeline.BuildLoopsFromCommands(variables);

        timeline.Loops.Count.ShouldBe(1);
        timeline.Loops[0].StartBeat.ShouldBe(0f);
        timeline.Loops[0].EndBeat.ShouldBe(4f);
    }

    [Fact]
    public void BuildLoopsFromCommands_Should_Use_Previous_Beat_When_Start_Command_Has_No_Beat()
    {
        var melody = new Melody { LengthInBeats = 1f };
        var variables = CreateVariables(((typeof(Melody), "_lead"), melody));

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
            Id = "verse",
            Type = TimelineCommandType.Start,
            TargetIds = new List<string> { "_lead" }
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Id = "verse",
            Type = TimelineCommandType.Stop,
            Beat = 8
        });

        timeline.BuildLoopsFromCommands(variables);

        timeline.Loops.Count.ShouldBe(2);
        timeline.Loops[0].StartBeat.ShouldBe(0f);
        timeline.Loops[0].EndBeat.ShouldBe(4f);
        timeline.Loops[1].StartBeat.ShouldBe(4f);
        timeline.Loops[1].EndBeat.ShouldBe(8f);
    }

    [Fact]
    public void BuildLoopsFromCommands_Should_Stop_Relative_To_Start_Beat_When_Configured()
    {
        var melody = new Melody { LengthInBeats = 1f };
        var variables = CreateVariables(((typeof(Melody), "_lead"), melody));

        var timeline = new Timeline();
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Start,
            Beat = 10,
            TargetIds = new List<string> { "_lead" }
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Type = TimelineCommandType.Stop,
            Beat = 4,
            IsBeatRelativeToStart = true,
            TargetIds = new List<string> { "_lead" }
        });

        timeline.BuildLoopsFromCommands(variables);

        timeline.Loops.Count.ShouldBe(1);
        timeline.Loops[0].StartBeat.ShouldBe(10f);
        timeline.Loops[0].EndBeat.ShouldBe(14f);
    }

    [Fact]
    public void BuildLoopsFromCommands_Should_Advance_Command_Cursor_After_Relative_Stop()
    {
        var melody = new Melody { LengthInBeats = 1f };
        var variables = CreateVariables(((typeof(Melody), "_lead"), melody));

        var timeline = new Timeline();
        timeline.Commands.Add(new TimelineCommand
        {
            Id = "intro",
            Type = TimelineCommandType.Start,
            Beat = 10,
            TargetIds = new List<string> { "_lead" }
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Id = "intro",
            Type = TimelineCommandType.Stop,
            Beat = 4,
            IsBeatRelativeToStart = true
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Id = "verse",
            Type = TimelineCommandType.Start,
            TargetIds = new List<string> { "_lead" }
        });
        timeline.Commands.Add(new TimelineCommand
        {
            Id = "verse",
            Type = TimelineCommandType.Stop,
            Beat = 2,
            IsBeatRelativeToStart = true
        });

        timeline.BuildLoopsFromCommands(variables);

        timeline.Loops.Count.ShouldBe(2);
        timeline.Loops[0].StartBeat.ShouldBe(10f);
        timeline.Loops[0].EndBeat.ShouldBe(14f);
        timeline.Loops[1].StartBeat.ShouldBe(14f);
        timeline.Loops[1].EndBeat.ShouldBe(16f);
    }

    private static RuntimeVariableTable CreateVariables(params ((Type type, string id) key, RuntimeObject value)[] entries)
    {
        var table = new RuntimeVariableTable();
        var dictionary = new Dictionary<(Type, string), RuntimeObject>();

        foreach (var (key, value) in entries)
        {
            dictionary[key] = value;
        }

        typeof(RuntimeVariableTable)
            .GetField("_variables", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(table, dictionary);

        return table;
    }
}