using Ast.NodeArchetypes;
using Runtime.Objects;
using Lexing.Tokens;
using Ast.Nodes.Floats;
using System.Globalization;

namespace Ast.Nodes.Patterns;

public class PatternNode : SymbolNode
{
	public float LengthInBeats;
	public List<string> PatternAndMelodyIds = new();
	public Pattern Pattern = new();

	public PatternNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.PatternKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture));
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = LengthInBeats + value; });

		while (Parser.TryConsumeIndent(1))
		{
			new ReferenceNode(this, this);
		}
	}

	protected override void Validate()
	{
		if (LengthInBeats <= 0)
		{
			Validator.AddError(this, $"Pattern: '{Id}'. Length cannot be <= 0");
		}
	}

	protected override void Evaluate()
	{
		this.Pattern.LengthInBeats = this.LengthInBeats;
	}
}