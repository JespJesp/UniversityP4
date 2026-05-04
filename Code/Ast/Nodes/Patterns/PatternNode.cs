using System.Globalization;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Patterns;

public class PatternNode : SymbolNode
{
	public Pattern Pattern = new();
	public float LengthInBeats;

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.Float, out string lengthValue);
		LengthInBeats = float.Parse(lengthValue, CultureInfo.InvariantCulture);

		parser.ConsumeToken(TokenType.Identifier, out string nameValue);
		Id = LengthInBeats + nameValue;

		while (parser.TryConsumeNewlineIndent(1))
		{
			parser.ParseChild(this, new ReferenceNode(this));
		}
	}

	public override void Validate(Validator validator)
	{
		if (LengthInBeats <= 0)
		{
			throw new Exception($"Pattern: '{Id}'. Length cannot be <= 0");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Pattern.LengthInBeats = LengthInBeats;
	}
}