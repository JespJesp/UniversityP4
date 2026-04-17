using Ast.NodeArchetypes;
using Ast.Nodes.Samples;
using Phases.Parsing;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferenceNode : Node
{
	public SampleReferencesNode SampleReferencesNode;
	public string ReferenceId = "";

	public SampleReferenceNode(SampleReferencesNode sampleReferencesNode)
	{
		this.SampleReferencesNode = sampleReferencesNode;
	}

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => ReferenceId = value);
	}

	public override void Annotate()
	{
		if (!SymbolTable.Contains<SampleNode>(ReferenceId))
		{
			throw new Exception($"Melody: '{SampleReferencesNode.MelodyNode.Id}'. The sample reference '{ReferenceId}' is not declared");
		}
	}

	public override void Evaluate()
	{
		Melody melody = SymbolTable.Get<MelodyNode>(SampleReferencesNode.MelodyNode.Id).Melody;
		Sample sample = SymbolTable.Get<SampleNode>(ReferenceId).Sample;
		melody.Samples.Add(sample);
	}
}

