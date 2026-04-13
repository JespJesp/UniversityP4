using System.Globalization;
using Ast.NodeArchetypes;
using Ast.Tables;
using Ast.Nodes.Melodies.Chords.Notes;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords;

public class ChordNode : BranchNode
{
	public ChordsNode ChordsNode;
	public float StartBeat;
	public float EndBeat;

	public ChordNode(Node parent, ChordsNode chordsNode) : base(parent)
	{
		this.ChordsNode = chordsNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Float, (value) => StartBeat = float.Parse(value, CultureInfo.InvariantCulture));
		Parser.ConsumeToken(TokenType.Float, (value) => EndBeat = float.Parse(value, CultureInfo.InvariantCulture));

		while (Parser.CursorToken.Type == TokenType.Identifier)
		{
			new NoteNode(this, this);
		}
	}

	protected override void Validate(SemanticSymbolTable symbols)
	{
		MelodyNode melodyNode = ChordsNode.MelodyNode;

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

