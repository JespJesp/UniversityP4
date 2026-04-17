using Ast.Tables;

namespace Runtime.Objects;

public class Timeline : RuntimeObject
{
	public int SampleRate = 48000;
	public int BeatsPerMinute = 120;
	public int BeatsPerBar = 4;
	public int BeatNoteValue = 4;

	public List<TimelineCommand> Commands = new();
	public List<Loop> Loops = new();

	public void BuildLoopsFromCommands(RuntimeVariableTable variables)
	{
		TimelineLoopBuilder.Build(this, variables);
	}
}