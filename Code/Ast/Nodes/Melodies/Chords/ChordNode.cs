using System.Globalization;
using Ast.Tables;
using Ast.Nodes.Melodies.Chords.Notes;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords;

public class ChordNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
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

	protected override void Validate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		MelodyNode melodyNode = ancestors.Get<MelodyNode>();

		if (EndBeat > melodyNode.LengthInBeats)
		{
			Validator.AddError(this, $"Melody: {melodyNode.Id}. Note end time {EndBeat} exceeds melody length {melodyNode.LengthInBeats}");
		}
		if (StartBeat < 0 || EndBeat < 0)
		{
			Validator.AddError(this, $"Melody: {melodyNode.Id}. Start time and end time must be positive: {StartBeat}-{EndBeat}");
		}
		if (StartBeat >= EndBeat)
		{
			Validator.AddError(this, $"Melody: {melodyNode.Id}. Start time must be less than end time: {StartBeat}-{EndBeat}");
		}
	}
}

