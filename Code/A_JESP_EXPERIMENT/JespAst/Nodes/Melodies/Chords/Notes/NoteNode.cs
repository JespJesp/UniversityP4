using JespAst.Tables;
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

	protected override void Evaluate(NodeTable localNodes, VariableTable localVariables)
	{
		Melody melody = localVariables.Get<Melody>(localNodes.Get<MelodyDeclarationNode>().Id);
		ChordNode chordNode = localNodes.Get<ChordNode>();

		Note note = new()
		{
			StartBeat = chordNode.StartBeat,
			EndBeat = chordNode.EndBeat,
			Pitch0 = new(this.Pitch)
		};
		melody.Notes.Add(note);
	}
}

