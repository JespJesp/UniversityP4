using System.Globalization;
using Ast.Tables;
using Runtime.Objects;
using Ast.Nodes.Melodies.Chords;
using Ast.NodeArchetypes;
using Ast.Nodes.Melodies.Samples;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies;

public class MelodyNode : VariableNode
{
	public float LengthInBeats;

	public Melody Melody = new();
	protected override RuntimeObject GetRuntimeObject() => this.Melody;

	public MelodyNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.MelodyKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture));
		Parser.ConsumeToken(TokenType.Identifier, (value) => Id = LengthInBeats + value);

		Parser.TryConsumeIndent(1);
		Dictionary<TokenType, Action> options = new()
		{
			{
				TokenType.SamplesKeyword,
				() => { new SampleReferencesNode(this, this); }
			},
			{
				TokenType.ChordsKeyword,
				() => { new ChordsNode(this, this); }
			}
		};
		Token[] optionSeparator = { new(TokenType.Newline), new(TokenType.Indent, "1") };
		Parser.HandleUniqueOptions(options, optionSeparator);
	}

	protected override void AdditionalValidation(SemanticSymbolTable symbols)
	{
		if (LengthInBeats <= 0)
		{
			Validator.AddError(this, $"Melody: '{Id}'. Length cannot be <= 0");
		}
	}

	protected override void AdditionalEvaluation(RuntimeVariableTable variables)
	{
		this.Melody.LengthInBeats = this.LengthInBeats;
	}
}

