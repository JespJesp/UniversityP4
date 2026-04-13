using Ast.NodeArchetypes;
using Ast.Nodes.Melodies;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Patterns;

public class ReferenceNode : BranchNode
{
	public PatternNode PatternNode;
	public string ReferenceId = "";

	public ReferenceNode(Node parent, PatternNode patterNode) : base(parent)
	{
		this.PatternNode = patterNode;
	}

	protected override void Parse()
	{
		string length = "";
		Parser.ConsumeToken(TokenType.Float, (value) => { length = value; });
		Parser.ConsumeToken(TokenType.Identifier, (value) => { ReferenceId = length + value; });
	}

	protected override void Validate()
	{
		if (!_symbolTable.Contains<PatternNode>(ReferenceId) 
			&& !_symbolTable.Contains<MelodyNode>(ReferenceId))
		{
			Validator.AddError(this, $"Pattern: '{ReferenceId}'. The pattern or melody reference '{ReferenceId}' is not declared");
		}
	}

	protected override void Evaluate()
	{
		Pattern pattern = _symbolTable.Get<PatternNode>(PatternNode.Id).Pattern;

		if (_symbolTable.TryGet(this.ReferenceId, out PatternNode childPatternNode))
		{
			pattern.Patterns.Add(childPatternNode.Pattern);
		}
		else if (_symbolTable.TryGet(this.ReferenceId, out MelodyNode childMelodyNode))
		{
			pattern.Melodies.Add(childMelodyNode.Melody);
		}
		else
		{
			throw new Exception($"Pattern references undefined ID '{this.ReferenceId}'");
		}
	}
}