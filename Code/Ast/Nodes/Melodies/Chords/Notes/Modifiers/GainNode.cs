using Ast.NodeArchetypes;
using Runtime.Objects;
using Lexing.Tokens;
using Ast.Nodes.Floats;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class GainNode : BranchNode
{
	public ModifiersNode ModifiersNode;
	public FloatExpressionNode Volume = new();

	public GainNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.GainKeyword);
		Volume = ParseChild(new FloatExpressionNode());
	}

	protected override void Validate()
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (Volume.Value < 0.0f)
		{
			throw new Exception($"Melody: '{melodyNode.Id}'. Note: '{noteNode.Pitch}'. Volume cannot be negative, but was: '{Volume.Value}'");
		}
	}

	protected override void Evaluate()
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Volume = Volume.Value;
	}
}

