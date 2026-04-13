using Ast.NodeArchetypes;
using Runtime.Objects;
using Lexing.Tokens;
using Ast.Nodes.Floats;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class PanNode : BranchNode
{
	public ModifiersNode ModifiersNode;
	public FloatExpressionNode Pan;

	public PanNode(Node parent, ModifiersNode modifiersNode) : base(parent)
	{
		this.ModifiersNode = modifiersNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.PanKeyword);
		Pan = new FloatExpressionNode(this);
	}

	protected override void Validate()
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (Pan.GetValue() < -1.0f || Pan.GetValue() > 1.0f)
		{
			Validator.AddError(this, $"Melody: '{melodyNode.Id}'. Note: '{noteNode.Pitch}'. Pan must be between -1 and 1, but was: {Pan.GetValue()}");
		}
	}

	protected override void Evaluate()
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Pan = Pan.GetValue();
	}
}

