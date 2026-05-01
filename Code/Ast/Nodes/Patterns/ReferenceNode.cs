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

	protected override void Validate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		parser.ConsumeToken(TokenType.Float, out string length);
		parser.ConsumeToken(TokenType.Identifier, out string name);
		ReferenceId = length + name;
	}

	public override void Annotate(Annotator annotator)
	{
		if (!SymbolTable.Contains<PatternNode>(ReferenceId)
			&& !SymbolTable.Contains<MelodyNode>(ReferenceId))
		{
			Validator.AddError(this, $"Pattern: '{ReferenceId}'. The pattern or melody reference '{ReferenceId}' is not declared");
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable localVariables)
	{
		Pattern pattern = localVariables.Get<Pattern>(ancestors.Get<PatternNode>().Id);

		if (SymbolTable.TryGet(ReferenceId, out PatternNode childPatternNode))
		{
			pattern.Patterns.Add(childPattern);
		}
		else if (SymbolTable.TryGet(ReferenceId, out MelodyNode childMelodyNode))
		{
			pattern.Melodies.Add(childMelody);
		}
		else
		{
			throw new Exception($"Pattern references undefined ID '{ReferenceId}'");
		}
	}
}