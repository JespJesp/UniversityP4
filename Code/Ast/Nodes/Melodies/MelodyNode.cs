using Runtime.Objects;
using Ast.Nodes.Melodies.Chords;
using Ast.NodeArchetypes;
using Ast.Nodes.Melodies.Samples;
using Lexing.Tokens;
using Ast.Nodes.Floats;
using System.Globalization;

namespace Ast.Nodes.Melodies;

public class MelodyNode : SymbolNode
{
	public float LengthInBeats;
	public Melody Melody = new();

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.MelodyKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture));
		Parser.ConsumeToken(TokenType.Identifier, (value) => Id = LengthInBeats + value);

		if (Parser.TryConsumeIndent(1))
		{
			Dictionary<TokenType, Action> options = new()
			{
				{
					TokenType.SamplesKeyword,
					() => { ParseChild(new SampleReferencesNode(this)); }
				},
				{
					TokenType.ChordsKeyword,
					() => { ParseChild(new ChordsNode(this)); }
				}
			};
			Token[] optionSeparator =
			{
				new(TokenType.Newline),
				new(TokenType.Indent, "1")
			};
			Parser.HandleUniqueOptions(options, optionSeparator);
		}
	}

	protected override void Validate()
	{
		if (LengthInBeats <= 0)
		{
			Validator.AddError(this, $"Melody: '{Id}'. Length cannot be <= 0");
		}
	}

	protected override void Evaluate()
	{
		this.Melody.LengthInBeats = this.LengthInBeats;
	}
}

