using Ast.Nodes.Floats;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;

namespace Ast.Nodes.Melodies.Samples.Modifiers;

public class HoldNode : Node
{
	public ModifiersNode ModifiersNode;
	public FloatExpressionNode HoldBeats = new();

	public HoldNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse(Parser parser)
	{
		HoldBeats = parser.ParseChild(this, new FloatExpressionNode());
	}

	public override void Validate(Validator validator)
	{
		if (HoldBeats.Value < 0.0f)
		{
			SampleReferenceNode sampleReferenceNode = ModifiersNode.SampleReferenceNode;
			MelodyNode melodyNode = sampleReferenceNode.SampleReferencesNode.MelodyNode;
			throw new Exception($"Melody: '{melodyNode.Id}'. Sample reference: '{sampleReferenceNode.ReferenceId}'. Hold '{HoldBeats.Value}' cannot be negative");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Sample sourceSampleClone = ModifiersNode.SampleReferenceNode.SourceSampleClone;
		sourceSampleClone.HoldBeats = HoldBeats.Value;
	}
}

