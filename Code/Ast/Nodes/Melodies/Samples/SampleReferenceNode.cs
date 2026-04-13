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

	protected override void Annotate()
	{
		if (!_symbolTable.Contains<Sample>(ReferenceId))
		{
			Annotator.AddError(this, $"Melody: '{SampleReferencesNode.MelodyNode.Id}'. The sample reference '{ReferenceId}' is not declared");
		}
	}

	protected override void Evaluate()
	{
		Melody melody = _symbolTable.Get<Melody>(SampleReferencesNode.MelodyNode.Id);
		Sample sample = _symbolTable.Get<Sample>(ReferenceId);
		melody.Samples.Add(sample);
	}
}

