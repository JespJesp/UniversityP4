using System.Globalization;
using Ast.NodeArchetypes;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Patterns;

public class PatternNode : SymbolNode
{
	public float LengthInBeats;
	public List<string> PatternAndMelodyIds = new();
	public Pattern Pattern = new();

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.PatternKeyword);
		parser.ConsumeToken(TokenType.Float, (value) => LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture));
		parser.ConsumeToken(TokenType.Identifier, (value) => { Id = LengthInBeats + value; });

		while (parser.TryConsumeIndent(1))
		{
			parser.ParseChild(this, new ReferenceNode(this));
		}
	}

	public override void Validate(Validator validator)
	{
		if (LengthInBeats <= 0)
		{
			throw new Exception($"Pattern: '{Id}'. Length cannot be <= 0.");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		this.Pattern.LengthInBeats = this.LengthInBeats;
	}
}