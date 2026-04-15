using Ast.NodeArchetypes;
using Parsing;
using Runtime.Objects;
using Lexing.Tokens;
using System.Globalization;

namespace Ast.Nodes.Patterns;

public class PatternNode : SymbolNode
{
	public float LengthInBeats;
	public List<string> PatternAndMelodyIds = new();
	public Pattern Pattern = new();

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.PatternKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture));
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = LengthInBeats + value; });

		while (Parser.TryConsumeIndent(1))
		{
			ParseChild(new ReferenceNode(this));
		}
	}

	protected override void Validate()
	{
		if (LengthInBeats <= 0)
		{
			throw new Exception($"Pattern: '{Id}'. Length cannot be <= 0.");
		}
	}

	protected override void Evaluate()
	{
		this.Pattern.LengthInBeats = this.LengthInBeats;
	}
}