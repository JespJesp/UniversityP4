using System.Globalization;
using Ast.Nodes.Melodies.Chords;
using Ast.Nodes.Melodies.Samples;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies;

public class MelodyNode : SymbolNode
{
	public Melody Melody = new();
	public float LengthInBeats;

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.Float, (value) => LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture));
		parser.ConsumeToken(TokenType.Identifier, (value) => Id = LengthInBeats + value);

		if (parser.TryConsumeIndent(1))
		{
			List<Func<bool>> options = new()
			{
				() => parser.TryConsumeToken(TokenType.Identifier, "samples", (value) => parser.ParseChild(this, new SampleReferencesNode(this))),
				() => parser.TryConsumeToken(TokenType.Identifier, "chords", (value) => parser.ParseChild(this, new ChordsNode(this))),
			};
			Token[] optionSeparator =
			{
				new(TokenType.Newline),
				new(TokenType.Indent, "1"),
			};
			parser.TryConsumeOptions(options, optionSeparator);
		}
	}

	public override void Validate(Validator validator)
	{
		if (LengthInBeats <= 0)
		{
			throw new Exception($"Melody: '{Id}'. Length cannot be <= 0");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		this.Melody.LengthInBeats = this.LengthInBeats;
	}
}

