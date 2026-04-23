using Ast.Nodes.Floats;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class PanNode : Node
{
	public ModifiersNode ModifiersNode;
	private FloatExpressionNode _pan = new();

	public PanNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse(Parser parser)
	{
		_pan = parser.ParseChild(this, new FloatExpressionNode());
	}

	public override void Validate(Validator validator)
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (_pan.Value < -1.0f || _pan.Value > 1.0f)
		{
			throw new Exception($"Melody: '{melodyNode.Id}'. Note: '{noteNode.PitchString}'. Pan '{_pan.Value}' must be between -1 and 1");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Pan = _pan.Value;
	}
}

