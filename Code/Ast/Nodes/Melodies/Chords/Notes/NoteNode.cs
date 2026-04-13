using Ast.NodeArchetypes;
using Ast.Tables;
using Runtime.Objects;
using Ast.Nodes.Melodies.Chords.Notes.Modifiers;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes;

public class NoteNode : BranchNode
{
	public ChordNode ChordNode;
	public string Pitch = "";
	public Note Note = new();

	public NoteNode(Node parent, ChordNode chordsNode) : base(parent)
	{
		this.ChordNode = chordsNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => Pitch = value);

		if (Parser.CursorToken.Type == TokenType.LeftParentheses)
		{
			new ModifiersNode(this, this);
		}
	}

	protected override void Evaluate(RuntimeVariableTable variables)
	{
		this.Note.StartBeat = ChordNode.StartBeat;
		this.Note.EndBeat = ChordNode.EndBeat;
		this.Note.Pitch = new(this.Pitch);

		Melody melody = ChordNode.ChordsNode.MelodyNode.Melody;
		melody.Notes.Add(Note);
	}
}

