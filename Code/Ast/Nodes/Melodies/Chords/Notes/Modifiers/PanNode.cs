using Ast.Nodes.Floats;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class PanNode : Node
{
	public ModifiersNode ModifiersNode;
	public FloatExpressionNode Pan = new();

	public PanNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.PanKeyword);
		Pan = parser.ParseChild(this, new FloatExpressionNode());
	}

	public override void Validate(Validator validator)
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (Pan.Value < -1.0f || Pan.Value > 1.0f)
		{
			throw new Exception($"Melody: '{melodyNode.Id}'. Note: '{noteNode.PitchString}'. Pan must be between -1 and 1, but was: '{Pan.Value}'.");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Pan = Pan.Value;
	}
}

