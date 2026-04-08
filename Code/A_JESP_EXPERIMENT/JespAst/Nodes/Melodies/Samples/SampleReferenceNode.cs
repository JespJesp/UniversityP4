using JespAst.Tables;
using JespAst.Nodes.Samples;
using JespRuntime.Objects;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Samples;

public class SampleReferenceNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => value = Id);
	}

	protected override void Annotate(NodeTable ancestors, SymbolTable symbols)
	{
		MelodyNode melodyNode = ancestors.Get<MelodyNode>();

		if (!symbols.Contains(typeof(SampleNode), Id))
		{
			Annotator.AddSemanticError($"Melody: '{melodyNode}'. The sample reference '{Id}' is not declared");
		}
	}

	protected override void Evaluate(NodeTable ancestors, VariableTable variables)
	{
		Melody melody = variables.Get<Melody>(ancestors.Get<MelodyNode>().Id);
		Sample sample = variables.Get<Sample>(Id);
		melody.Samples.Add(sample);
	}
}

