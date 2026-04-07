using JespAst.Nodes.Samples;
using JespRuntime;
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

	protected override void Annotate(HashSet<(Type, string)> localSymbolTable)
	{
		if (!localSymbolTable.Contains((typeof(SampleDeclaration), Id)))
		{
			AddSemanticError($"The sample reference '{Id}' is not declared");
		}
	}

	protected override void Evaluate(LocalVariables localSymbolTable)
	{
		Melody melody = localSymbolTable.Get<Melody>();
		Sample sample = GlobalVariables.Get<Sample>(Id);
		melody.Samples.Add(sample);
	}
}

