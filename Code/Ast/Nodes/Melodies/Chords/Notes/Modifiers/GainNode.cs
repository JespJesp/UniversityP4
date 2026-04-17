using Ast.NodeArchetypes;
using Ast.Nodes.Floats;
using Phases.Parsing;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class GainNode : Node
{
	public ModifiersNode ModifiersNode;
	public FloatExpressionNode Volume = new();

	public GainNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.GainKeyword);
		Volume = Parser.ParseChild(this, new FloatExpressionNode());
	}

	public override void Validate()
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (Volume.Value < 0.0f)
		{
			throw new Exception($"Melody: '{melodyNode.Id}'. Note: '{noteNode.PitchString}'. Volume cannot be negative, but was: '{Volume.Value}'");
		}
	}

	public override void Evaluate()
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Volume = Volume.Value;
	}
}

