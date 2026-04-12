using Ast.Tables;
using Ast.Nodes.Samples;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferenceNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => Id = value);
	}

	protected override void Annotate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		MelodyNode melodyNode = ancestors.Get<MelodyNode>();

		if (!symbols.Contains(typeof(SampleNode), Id))
		{
			Annotator.AddError(this, $"Melody: '{melodyNode.Id}'. The sample reference '{Id}' is not declared");
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		Melody melody = variables.Get<Melody>(ancestors.Get<MelodyNode>().Id);
		Sample sample = variables.Get<Sample>(Id);
		melody.Samples.Add(sample);
	}
}

