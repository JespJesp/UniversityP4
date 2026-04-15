using Ast.NodeArchetypes;
using Ast.Nodes.Floats;
using Phases.Parsing;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class PanNode : BranchNode
{
	public ModifiersNode ModifiersNode;
	public FloatExpressionNode Pan = new();

	public PanNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.PanKeyword);
		Pan = ParseChild(new FloatExpressionNode());
	}

	protected override void Validate()
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (Pan.Value < -1.0f || Pan.Value > 1.0f)
		{
			throw new Exception($"Melody: '{melodyNode.Id}'. Note: '{noteNode.Pitch}'. Pan must be between -1 and 1, but was: '{Pan.Value}'.");
		}
	}

	protected override void Evaluate()
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Pan = Pan.Value;
	}
}

