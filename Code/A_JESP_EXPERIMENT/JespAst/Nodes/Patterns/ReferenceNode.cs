using JespAst.Tables;
using JespAst.Nodes.Melodies;
using JespRuntime.Objects;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Patterns;

public class ReferenceNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";

	protected override void Parse()
	{
		string length = "";
		Parser.ConsumeToken(TokenType.Float, (value) => { length = value; });
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = length + value; });
	}

	protected override void Annotate(NodeTable ancestors, SymbolTable symbols)
	{
		if (!symbols.Contains(typeof(PatternNode), Id) && !symbols.Contains(typeof(MelodyNode), Id))
		{
			Annotator.AddSemanticError($"Pattern: '{Id}'. The pattern or melody reference '{Id}' is not declared");
		}
	}

	protected override void Evaluate(NodeTable ancestors, VariableTable localVariables)
	{
		Pattern pattern = localVariables.Get<Pattern>(ancestors.Get<PatternNode>().Id);

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