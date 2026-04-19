using Ast.Nodes.Floats;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;

namespace Ast.Nodes.Melodies.Samples.Modifiers;

public class SustainNode : Node
{
	public ModifiersNode ModifiersNode;
	public FloatExpressionNode SustainLevel = new();

	public SustainNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse(Parser parser)
	{
		SustainLevel = parser.ParseChild(this, new FloatExpressionNode());
	}

	public override void Validate(Validator validator)
	{
		if (SustainLevel.Value < 0.0f || SustainLevel.Value > 1.0f)
		{
			SampleReferenceNode sampleReferenceNode = ModifiersNode.SampleReferenceNode;
			MelodyNode melodyNode = sampleReferenceNode.SampleReferencesNode.MelodyNode;
			
			throw new Exception($"Melody: '{melodyNode.Id}'. Sample reference: '{sampleReferenceNode.ReferenceId}'. Sustain '{SustainLevel.Value}' must be between 0 and 1");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Sample sourceSampleClone = ModifiersNode.SampleReferenceNode.SourceSampleClone;
		sourceSampleClone.SustainLevel = SustainLevel.Value;
	}
}

