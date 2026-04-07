using JespRuntime;
using JespRuntime.Nodes;
using JespAst.Nodes.Melodies.Chords.Notes.Modifiers;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Chords.Notes;

public class NoteNode(Node parent) : Node(parent)
{
	public string Pitch = "";

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => Pitch = value);

		if (Parser.CurrentToken.Type == TokenType.LeftParentheses)
		{
			new ModifiersNode(this);
		}
	}

	protected override void Evaluate(LocalVariables localSymbolTable)
	{
		Melody melody = localSymbolTable.Get<Melody>();
		ChordNode chordNode = localSymbolTable.Get<ChordNode>();

		Note note = new()
		{
			StartBeat = chordNode.StartBeat,
			EndBeat = chordNode.EndBeat,
			Pitch0 = new(this.Pitch)
		};
		localSymbolTable.Add(note);
		melody.Notes.Add(note);
	}
}

