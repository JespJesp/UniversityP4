using Ast.NodeArchetypes;
using Ast.Tables;
using Ast.Nodes.Samples;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferenceNode : BranchNode
{
	public SampleReferencesNode SampleReferencesNode;
	public string Id = "";

	public SampleReferenceNode(Node parent, SampleReferencesNode sampleReferencesNode) : base(parent)
	{
		this.SampleReferencesNode = sampleReferencesNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => Id = value);
	}

	protected override void Validate(SemanticSymbolTable symbols)
	{
		if (!symbols.Contains(typeof(SampleNode), Id))
		{
			Validator.AddError(this, $"Melody: '{SampleReferencesNode.MelodyNode.Id}'. The sample reference '{Id}' is not declared");
		}
	}

	protected override void Evaluate(RuntimeVariableTable variables)
	{
		Melody melody = variables.Get<Melody>(SampleReferencesNode.MelodyNode.Id);
		Sample sample = variables.Get<Sample>(Id);
		melody.Samples.Add(sample);
	}
}

