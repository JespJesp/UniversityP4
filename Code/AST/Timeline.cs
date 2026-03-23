namespace AST;

public class Timeline
{
	public TimelineSettings Settings = new();
	public List<TimelineCommand> Commands = new();
}

public class TimelineSettings
{
	public string TimeSignature = "4/4";
	public int Bpm = 120;
}

public class TimelineCommand
{
	public string Id = "";
	public TimelineCommandType Type;
	public int? Beat;
	public List<string> PatternNames = new();
}

public enum TimelineCommandType
{
	Start,
	Stop
}
