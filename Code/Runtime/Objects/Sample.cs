namespace Runtime.Objects;

public class Sample : RuntimeObject
{
	public string FilePath = "";
	public Pitch ReferencePitch = new();
	public float DelayBeats = 0.0f;
	public float AttackBeats = 0.0f;
	public float HoldBeats = 0.0f;
	public float DecayBeats = 0.0f;
	public float SustainLevel = 1.0f;
	public float ReleaseBeats = 0.0f;

	public Sample Clone()
	{
		return new()
		{
			FilePath = this.FilePath,
			ReferencePitch = this.ReferencePitch.Clone(),
			DelayBeats = this.DelayBeats,
			AttackBeats = this.AttackBeats,
			HoldBeats = this.HoldBeats,
			DecayBeats = this.DecayBeats,
			SustainLevel = this.SustainLevel,
			ReleaseBeats = this.ReleaseBeats,
		};
	}
}