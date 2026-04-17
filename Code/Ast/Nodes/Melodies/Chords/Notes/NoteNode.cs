using Ast.NodeArchetypes;
using Ast.Nodes.Melodies.Chords.Notes.Modifiers;
using Phases.Parsing;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes;

public class NoteNode : BranchNode
{
	public ChordNode ChordNode;
	public string PitchString = "";
	public Note Note = new();

	public NoteNode(ChordNode chordsNode)
	{
		this.ChordNode = chordsNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => PitchString = value);

		if (Parser.CursorToken.Type == TokenType.LeftParentheses)
		{
			ParseChild(new ModifiersNode(this));
		}
	}

	protected override void Annotate()
	{
		Pitch.FromString(this.PitchString);
	}

	protected override void Evaluate()
	{
		this.Note.StartBeat = ChordNode.StartBeat.Value;
		this.Note.EndBeat = ChordNode.EndBeat.Value;
		this.Note.Pitch = Pitch.FromString(this.PitchString);

		Melody melody = ChordNode.ChordsNode.MelodyNode.Melody;
		melody.Notes.Add(Note);
	}
}

