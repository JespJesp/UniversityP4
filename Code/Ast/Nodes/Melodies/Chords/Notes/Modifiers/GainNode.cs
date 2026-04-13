using System.Globalization;
using Ast.NodeArchetypes;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class GainNode : BranchNode
{
	public ModifiersNode ModifiersNode;
	public float Volume = 1;

	public GainNode(Node parent, ModifiersNode modifiersNode) : base(parent)
	{
		this.ModifiersNode = modifiersNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.GainKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => Volume = float.Parse(value, CultureInfo.InvariantCulture));
	}

	protected override void Annotate()
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (Volume < 0.0f)
		{
			Annotator.AddError(this, $"Melody: '{melodyNode}'. Note: '{noteNode.Pitch}'. Volume cannot be negative, but was: {Volume}");
		}
	}

	protected override void Evaluate()
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Volume = Volume;
	}
}

