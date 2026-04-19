using Ast;

namespace Runtime.Objects.Timeline;

// TODO: Look through all of this Timeline stuff

public class Timeline : RuntimeObject
{
	public int SampleRate = 48000;
	public float BeatsPerMinute = 120;
	public float BeatsPerBar = 4;
	public float BeatNoteValue = 4;

	public List<TimelineCommand> Commands = new();
	public List<Loop> Loops = new();

	public void BuildLoopsFromCommands(SymbolTable globalSymbols)
	{
		TimelineLoopBuilder.Build(this, globalSymbols);
	}
}