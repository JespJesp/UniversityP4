using System.Globalization;
using Ast.NodeArchetypes;
using Ast.Nodes.Melodies.Chords;
using Ast.Nodes.Melodies.Samples;
using Phases.Parsing;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies;

public class MelodyNode : SymbolNode
{
	public float LengthInBeats;
	public Melody Melody = new();

	public override void CascadeParse()
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
					() => { Parser.ParseChild(this, new SampleReferencesNode(this)); }
				},
				{
					TokenType.ChordsKeyword,
					() => { Parser.ParseChild(this, new ChordsNode(this)); }
				}
			};
			Token[] optionSeparator =
			{
				new(TokenType.Newline),
				new(TokenType.Indent, "1")
			};
			Parser.AllowUniqueOptions(options, optionSeparator);
		}
	}

	public override void Validate()
	{
		if (LengthInBeats <= 0)
		{
			throw new Exception($"Melody: '{Id}'. Length cannot be <= 0");
		}
	}

	public override void Evaluate()
	{
		this.Melody.LengthInBeats = this.LengthInBeats;
	}
}

