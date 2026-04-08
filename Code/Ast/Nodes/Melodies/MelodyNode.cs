using System.Globalization;
using Ast.Tables;
using Runtime.Objects;
using Ast.Nodes.Melodies.Chords;
using Ast.Nodes.Melodies.Samples;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies;

public class MelodyNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";
	public float LengthInBeats;
	public Melody Melody0;

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
				() => { new SampleReferencesNode(this); }
			},
			{
				TokenType.NotesKeyword,
				() => { new ChordsNode(this); }
			}
		};
		Token[] optionSeparator = { new(TokenType.Newline), new(TokenType.Indent, "1") };
		Parser.HandleUniqueOptions(options, optionSeparator);
	}

	protected override void Annotate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		symbols.Add(this, Id);

		if (LengthInBeats <= 0)
		{
			Annotator.AddSemanticError(this, $"Melody: '{Id}'. Length cannot be <= 0");
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		this.Melody0 = new()
		{
			LengthInBeats = this.LengthInBeats
		};
		variables.Upsert(this.Melody0, Id);
	}
}

