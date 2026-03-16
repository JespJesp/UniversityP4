namespace AST;

public class Pattern
{
	public Song ParentSong;
	public int Length;
	public string Id = "";
	public List<string> SampleIds = new();
	public List<Note> Notes = new();

	public Pattern(Song parentSong)
	{
		ParentSong = parentSong;
	}
}