using Ast.NodeArchetypes;
using Ast.Nodes.Samples;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferenceNode : BranchNode
{
	public SampleReferencesNode SampleReferencesNode;
	public string ReferenceId = "";

	public SampleReferenceNode(Node parent, SampleReferencesNode sampleReferencesNode) : base(parent)
	{
		this.SampleReferencesNode = sampleReferencesNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => ReferenceId = value);
	}

	protected override void Validate()
	{
		if (!_symbolTable.Contains<SampleNode>(ReferenceId))
		{
			Validator.AddError(this, $"Melody: '{SampleReferencesNode.MelodyNode.Id}'. The sample reference '{ReferenceId}' is not declared");
		}
	}

	protected override void Evaluate()
	{
		Melody melody = _symbolTable.Get<MelodyNode>(SampleReferencesNode.MelodyNode.Id).Melody;
		Sample sample = _symbolTable.Get<SampleNode>(ReferenceId).Sample;
		melody.Samples.Add(sample);
	}
}

