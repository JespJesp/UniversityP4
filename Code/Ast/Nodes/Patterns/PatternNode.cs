using System.Globalization;
using Ast.NodeArchetypes;
using Phases.Parsing;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Patterns;

public class PatternNode : SymbolNode
{
	public float LengthInBeats;
	public List<string> PatternAndMelodyIds = new();
	public Pattern Pattern = new();

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.PatternKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture));
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = LengthInBeats + value; });

		while (Parser.TryConsumeIndent(1))
		{
			Parser.ParseChild(this, new ReferenceNode(this));
		}
	}

	public override void Validate()
	{
		if (LengthInBeats <= 0)
		{
			throw new Exception($"Pattern: '{Id}'. Length cannot be <= 0.");
		}
	}

	public override void Evaluate()
	{
		this.Pattern.LengthInBeats = this.LengthInBeats;
	}
}