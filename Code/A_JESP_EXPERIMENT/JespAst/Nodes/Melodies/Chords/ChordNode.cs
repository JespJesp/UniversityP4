using System.Globalization;
using JespRuntime;
using JespRuntime.Nodes;
using JespAst.Nodes.Melodies.Chords.Notes;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Chords;

public class ChordNode(Node parent) : Node(parent)
{
	public float StartBeat;
	public float EndBeat;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Float, (value) => StartBeat = float.Parse(value, CultureInfo.InvariantCulture));
		Parser.ConsumeToken(TokenType.Float, (value) => EndBeat = float.Parse(value, CultureInfo.InvariantCulture));

		while (Parser.CurrentToken.Type == TokenType.Identifier)
		{
			new NoteNode(this);
		}
	}

	protected override void Annotate(HashSet<(Type, string)> localSymbolTable)
	{
		if (StartBeat < 0 || EndBeat < 0)
		{
			AddSemanticError($"Start time and end time must be positive: {StartBeat}-{EndBeat}");
		}
		if (StartBeat >= EndBeat)
		{
			AddSemanticError($"Start time must be less than end time: {StartBeat}-{EndBeat}");
		}
		if (EndBeat > ParentMelody.LengthInBeats)
		{
			AddSemanticError($"Note end time {EndBeat} exceeds melody length {ParentMelody.LengthInBeats}");
		}
	}

	protected override void Evaluate(LocalVariables localSymbolTable)
	{
		localSymbolTable.Add(this);
	}
}

