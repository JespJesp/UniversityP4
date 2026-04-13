using Ast.NodeArchetypes;
using Runtime.Objects;
using Lexing.Tokens;
using Ast.Nodes.Floats;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class GainNode : BranchNode
{
	public ModifiersNode ModifiersNode;
	public FloatExpressionNode Volume;

	public GainNode(Node parent, ModifiersNode modifiersNode) : base(parent)
	{
		this.ModifiersNode = modifiersNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.GainKeyword);
		Volume = new FloatExpressionNode(this);
	}

	protected override void Validate()
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (Volume.GetValue() < 0.0f)
		{
			Validator.AddError(this, $"Melody: '{melodyNode.Id}'. Note: '{noteNode.Pitch}'. Volume cannot be negative, but was: {Volume.GetValue()}");
		}
	}

	protected override void Evaluate()
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Volume = Volume.GetValue();
	}
}

