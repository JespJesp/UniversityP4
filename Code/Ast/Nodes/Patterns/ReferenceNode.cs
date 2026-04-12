using Ast.Tables;
using Ast.Nodes.Melodies;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Patterns;

public class ReferenceNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string ReferenceId = "";

	protected override void Parse()
	{
		string length = "";
		Parser.ConsumeToken(TokenType.Float, (value) => { length = value; });
		Parser.ConsumeToken(TokenType.Identifier, (value) => { ReferenceId = length + value; });
	}

	protected override void Annotate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		if (!symbols.Contains(typeof(PatternNode), ReferenceId) && !symbols.Contains(typeof(MelodyNode), ReferenceId))
		{
			Annotator.AddError(this, $"Pattern: '{ReferenceId}'. The pattern or melody reference '{ReferenceId}' is not declared");
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable localVariables)
	{
		Pattern pattern = localVariables.Get<Pattern>(ancestors.Get<PatternNode>().Id);

		if (localVariables.TryGet(this.ReferenceId, out Pattern childPattern))
		{
			pattern.Patterns.Add(childPattern);
		}
		else if (localVariables.TryGet(this.ReferenceId, out Melody childMelody))
		{
			pattern.Melodies.Add(childMelody);
		}
		else
		{
			throw new Exception($"Pattern references undefined ID '{this.ReferenceId}'");
		}
	}
}