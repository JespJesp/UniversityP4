using Ast.Tables;
using Runtime.Objects;
using Ast.Nodes.Melodies.Chords.Notes.Modifiers;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes;

public class NoteNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Pitch = "";
	public Note Note0;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => Pitch = value);

		if (Parser.CurrentToken.Type == TokenType.LeftParentheses)
		{
			new ModifiersNode(this);
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		Melody melody = ancestors.Get<MelodyNode>().Melody0;
		ChordNode chordNode = ancestors.Get<ChordNode>();

		this.Note0 = new()
		{
			StartBeat = chordNode.StartBeat,
			EndBeat = chordNode.EndBeat,
			Pitch0 = new(this.Pitch)
		};
		melody.Notes.Add(Note0);
	}
}

