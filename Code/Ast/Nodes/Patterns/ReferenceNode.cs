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

	protected override void Annotate()
	{
		if (!_symbolTable.Contains<Pattern>(ReferenceId) 
			&& !_symbolTable.Contains<Melody>(ReferenceId))
		{
			Annotator.AddError(this, $"Pattern: '{ReferenceId}'. The pattern or melody reference '{ReferenceId}' is not declared");
		}
	}

	protected override void Evaluate()
	{
		Pattern pattern = _symbolTable.Get<Pattern>(PatternNode.Id);

		if (_symbolTable.TryGet(this.ReferenceId, out Pattern childPattern))
		{
			pattern.Patterns.Add(childPattern);
		}
		else if (_symbolTable.TryGet(this.ReferenceId, out Melody childMelody))
		{
			pattern.Melodies.Add(childMelody);
		}
		else
		{
			throw new Exception($"Pattern references undefined ID '{this.ReferenceId}'");
		}
	}
}