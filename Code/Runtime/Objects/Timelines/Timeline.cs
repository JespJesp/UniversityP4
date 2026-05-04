namespace Runtime.Objects.Timelines;

public class Timeline : IRuntimeObject
{
	public int SampleRate = 48000;
	public float BeatsPerMinute = 120;
	public float BeatsPerBar = 4;
	public float BeatNoteValue = 4;

	public List<TimelineCommand> Commands = new();
}