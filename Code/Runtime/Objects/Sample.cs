namespace Runtime.Objects;

public class Sample : IRuntimeObject
{
	public string FilePath = "";
	public Pitch ReferencePitch = new("c5"); // Default value
	public float DelayBeats = 0.0f;
	public float AttackBeats = 0.0f;
	public float HoldBeats = 0.0f;
	public float DecayBeats = 0.0f;
	public float SustainLevel = 1.0f;
	public float ReleaseBeats = 0.0f;
}