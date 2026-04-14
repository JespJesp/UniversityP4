using Ast.NodeArchetypes;
using Runtime.Objects;
using Ast.Nodes.Melodies.Chords.Notes.Modifiers;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes;

public class NoteNode : BranchNode
{
	public ChordNode ChordNode;
	public string Pitch = "";
	public Note Note = new();

	public NoteNode(ChordNode chordsNode)
	{
		this.ChordNode = chordsNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => Pitch = value);

		if (Parser.CursorToken.Type == TokenType.LeftParentheses)
		{
			ParseChild(new ModifiersNode(this));
		}
	}

	protected override void Evaluate()
	{
		this.Note.StartBeat = ChordNode.StartBeat.GetValue();
		this.Note.EndBeat = ChordNode.EndBeat.GetValue();
		this.Note.Pitch = new(this.Pitch);

		Melody melody = ChordNode.ChordsNode.MelodyNode.Melody;
		melody.Notes.Add(Note);
	}
}

