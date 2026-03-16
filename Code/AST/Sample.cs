namespace AST;

public class Sample
{
	public Song ParentSong;
	public string Id = "";
	public string FilePath = "";
	public Pitch ReferencePitch = new("c5"); // Default

	public Sample(Song parentSong)
	{
		ParentSong = parentSong;
	}
}