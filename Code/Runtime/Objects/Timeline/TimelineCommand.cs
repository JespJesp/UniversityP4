namespace Runtime.Objects;

public class TimelineCommand
{
	public string Id = "";
	public TimelineCommandType Type;
	public float? Beat;
	public List<string> TargetIds = new();
	public float GainMultiplier = 1.0f;
	public float PitchShiftHalfsteps = 0.0f;
	public float PanOffset = 0.0f;
}

public enum TimelineCommandType
{
	Start,
	Stop
}
