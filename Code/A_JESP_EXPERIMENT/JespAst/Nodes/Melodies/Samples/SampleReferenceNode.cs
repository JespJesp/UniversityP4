using JespAst.Tables;
using JespAst.Nodes.Samples;
using JespRuntime.Nodes;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Samples;

public class SampleReferenceNode(Node parent) : Node(parent)
{
	public string Id = "";

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => value = Id);
	}

	protected override void Annotate(NodeTable localNodes, SymbolTable localSymbols)
	{
		if (!localSymbols.Contains(typeof(SampleDeclaration), Id))
		{
			AddSemanticError($"The sample reference '{Id}' is not declared");
		}
	}

	protected override void Evaluate(NodeTable localNodes, VariableTable localVariables)
	{
		Melody melody = localVariables.Get<Melody>(localNodes.Get<MelodyDeclarationNode>().Id);
		Sample sample = localVariables.Get<Sample>(Id);
		melody.Samples.Add(sample);
	}
}

