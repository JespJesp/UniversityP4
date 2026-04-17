using Ast.NodeArchetypes;
using Ast.Nodes.Melodies;
using Phases.Parsing;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Patterns;

public class ReferenceNode : Node
{
	public PatternNode PatternNode;
	public string ReferenceId = "";

	public ReferenceNode(PatternNode patterNode)
	{
		this.PatternNode = patterNode;
	}

	public override void CascadeParse()
	{
		string length = "";
		Parser.ConsumeToken(TokenType.Float, (value) => { length = value; });
		Parser.ConsumeToken(TokenType.Identifier, (value) => { ReferenceId = length + value; });
	}

	public override void Annotate()
	{
		if (!SymbolTable.Contains<PatternNode>(ReferenceId)
			&& !SymbolTable.Contains<MelodyNode>(ReferenceId))
		{
			throw new Exception($"Pattern: '{ReferenceId}'. The pattern or melody reference '{ReferenceId}' is not declared");
		}
	}

	public override void Evaluate()
	{
		Pattern pattern = SymbolTable.Get<PatternNode>(PatternNode.Id).Pattern;

		if (SymbolTable.TryGet(this.ReferenceId, out PatternNode childPatternNode))
		{
			pattern.Patterns.Add(childPatternNode.Pattern);
		}
		else if (SymbolTable.TryGet(this.ReferenceId, out MelodyNode childMelodyNode))
		{
			pattern.Melodies.Add(childMelodyNode.Melody);
		}
		else
		{
			throw new Exception($"Pattern references undefined ID '{this.ReferenceId}'");
		}
	}
}