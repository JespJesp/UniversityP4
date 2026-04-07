using System.Globalization;
using JespAst.Tables;
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

	protected override void Annotate(NodeTable localNodes, SymbolTable localSymbols)
	{
		MelodyDeclarationNode melodyDeclarationNode = localNodes.Get<MelodyDeclarationNode>();

		if (EndBeat > melodyDeclarationNode.LengthInBeats)
		{
			AddSemanticError($"Note end time {EndBeat} exceeds melody length {melodyDeclarationNode.LengthInBeats}");
		}
		if (StartBeat < 0 || EndBeat < 0)
		{
			AddSemanticError($"Start time and end time must be positive: {StartBeat}-{EndBeat}");
		}
		if (StartBeat >= EndBeat)
		{
			AddSemanticError($"Start time must be less than end time: {StartBeat}-{EndBeat}");
		}
	}
}

