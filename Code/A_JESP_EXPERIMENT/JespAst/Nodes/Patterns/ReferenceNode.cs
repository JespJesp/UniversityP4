using JespAst.Tables;
using JespAst.Nodes.Melodies;
using JespRuntime.Nodes;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Patterns;

public class ReferenceNode(Node parent) : Node(parent)
{
	public string Id = "";

	protected override void Parse()
	{
		string length = "";
		Parser.ConsumeToken(TokenType.Float, (value) => { length = value; });
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = length + value; });
	}

	protected override void Annotate(NodeTable localNodes, SymbolTable localSymbols)
	{
		if (!localSymbols.Contains(typeof(PatternDeclarationNode), Id) && !localSymbols.Contains(typeof(MelodyDeclarationNode), Id))
		{
			AddSemanticError($"The pattern or melody reference '{Id}' is not declared");
		}
	}

	protected override void Evaluate(NodeTable localNodes, VariableTable localVariables)
	{
		Pattern pattern = localVariables.Get<Pattern>(localNodes.Get<PatternDeclarationNode>().Id);

		if (localVariables.TryGet(this.Id, out Pattern childPattern))
		{
			pattern.Patterns.Add(childPattern);
		}
		else if (localVariables.TryGet(this.Id, out Melody childMelody))
		{
			pattern.Melodies.Add(childMelody);
		}
		else
		{
			throw new Exception($"Pattern references undefined ID '{this.Id}'");
		}
	}
}