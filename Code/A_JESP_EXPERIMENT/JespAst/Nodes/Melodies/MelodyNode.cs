using System.Globalization;
using JespAst.Tables;
using JespRuntime.Objects;
using JespAst.Nodes.Melodies.Chords;
using JespAst.Nodes.Melodies.Samples;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies;

public class MelodyNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";
	public float LengthInBeats;
	public Melody Melody0;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Float, (value) => LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture));
		Parser.ConsumeToken(TokenType.Identifier, (value) => Id = value);

		Dictionary<TokenType, Action> options = new()
		{
			{
				TokenType.SampleKeyword,
				() => { new SampleReferencesNode(this); }
			},
			{
				TokenType.NotesKeyword,
				() => { new ChordsNode(this); }
			}
		};
		Token optionSeparator = new(TokenType.Indent, "2");
		Parser.TryConsumeUniqueOptions(options, optionSeparator);
	}

	protected override void Annotate(NodeTable ancestors, SymbolTable symbols)
	{
		if (LengthInBeats <= 0)
		{
			Annotator.AddSemanticError($"Melody: '{Id}'. Length cannot be <= 0");
		}
	}

	protected override void Evaluate(NodeTable ancestors, VariableTable variables)
	{
		this.Melody0 = new()
		{
			LengthInBeats = this.LengthInBeats
		};
		variables.Upsert(this.Melody0, Id);
	}
}

