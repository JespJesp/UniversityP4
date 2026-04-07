using JespAst.Nodes.Melodies;
using JespRuntime;
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

	protected override void Annotate(HashSet<(Type, string)> localSymbolTable)
	{
		if (!localSymbolTable.Contains((typeof(PatternDeclarationNode), Id)) && !localSymbolTable.Contains((typeof(MelodyDeclarationNode), Id)))
		{
			AddSemanticError($"The pattern or melody reference '{Id}' is not declared");
		}
	}

	protected override void Evaluate(LocalVariables localSymbolTable)
	{
		Pattern pattern = localSymbolTable.Get<Pattern>();

		if (GlobalVariables.TryGet(this.Id, out Pattern childPattern))
		{
			pattern.Patterns.Add(childPattern);
		}
		else if (GlobalVariables.TryGet(this.Id, out Melody childMelody))
		{
			pattern.Melodies.Add(childMelody);
		}
		else
		{
			throw new Exception($"Pattern references undefined ID '{this.Id}'");
		}
	}
}