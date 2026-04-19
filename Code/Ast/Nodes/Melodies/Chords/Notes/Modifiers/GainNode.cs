using Ast.Nodes.Floats;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
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

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.Identifier, "gain");
		Volume = parser.ParseChild(this, new FloatExpressionNode());
	}

	public override void Validate(Validator validator)
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (Volume.Value < 0.0f)
		{
			throw new Exception($"Melody: '{melodyNode.Id}'. Note: '{noteNode.PitchString}'. Volume '{Volume.Value}' cannot be negative");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Volume = Volume.Value;
	}
}

