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
		parser.ConsumeToken(TokenType.Float, out string lengthValue);
		LengthInBeats = float.Parse(lengthValue, CultureInfo.InvariantCulture);

		parser.ConsumeToken(TokenType.Identifier, out string nameValue);
		Id = LengthInBeats + nameValue;

		if (parser.TryConsumeNewlineIndent(1))
		{
			parser.TryConsumeOptions
			(
				new()
				{
					(
						() => parser.TryConsumeToken(TokenType.Identifier, "samples"),
						() => parser.ParseChild(this, new SampleReferencesNode(this))
					),
					(
						() => parser.TryConsumeToken(TokenType.Identifier, "chords"),
						() => parser.ParseChild(this, new ChordsNode(this))
					),
				},
				[
					new(TokenType.Newline),
					new(TokenType.Indent, "1"),
				]
			);
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
		Melody.LengthInBeats = LengthInBeats;
	}
}

