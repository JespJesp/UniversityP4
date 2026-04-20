using Ast.Nodes.Melodies;
using Phases.Annotation;
using Phases.Evaluation;
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

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.Float, out string length);
		parser.ConsumeToken(TokenType.Identifier, out string name);
		this.ReferenceId = length + name;
	}

	public override void Annotate(Annotator annotator)
	{
		if (!SymbolTable.Contains<PatternNode>(ReferenceId)
			&& !SymbolTable.Contains<MelodyNode>(ReferenceId))
		{
			throw new Exception($"Pattern: '{PatternNode.Id}'. The pattern or melody reference '{ReferenceId}' is not declared");
		}
	}

	public override void Evaluate(Evaluator evaluator)
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