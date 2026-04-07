using System.Globalization;
using JespRuntime;
using JespRuntime.Nodes;
using JespAst.Nodes.Melodies.Chords;
using JespAst.Nodes.Melodies.Samples;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies;

public class MelodyDeclarationNode(Node parent) : Node(parent)
{
	public string Id = "";
	public float LengthInBeats;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Float, (value) => LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture));
		Parser.ConsumeToken(TokenType.Identifier, (value) => Id = value);

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
		Token optionSeparator = new(TokenType.Indent, "2");
		Parser.TryConsumeUniqueOptions(options, optionSeparator);
	}

	protected override void Annotate(HashSet<(Type, string)> localSymbolTable)
	{
		localSymbolTable.Add((typeof(MelodyDeclarationNode), Id));

		if (string.IsNullOrWhiteSpace(Id))
		{
			AddSemanticError("ID cannot be empty");
		}
		if (LengthInBeats <= 0)
		{
			AddSemanticError("Length cannot be <= 0");
		}
	}

	protected override void Evaluate(LocalVariables localSymbolTable)
	{
		Melody melody = new()
		{
			LengthInBeats = this.LengthInBeats
		};
		localSymbolTable.Add(melody);
		GlobalVariables.Add(melody, Id);
	}
}

