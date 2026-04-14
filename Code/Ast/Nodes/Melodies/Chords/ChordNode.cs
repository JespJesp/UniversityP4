using Ast.NodeArchetypes;
using Ast.Nodes.Floats;
using Ast.Nodes.Melodies.Chords.Notes;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords;

public class ChordNode : BranchNode
{
	public ChordsNode ChordsNode;
	public FloatExpressionNode StartBeat;
	public FloatExpressionNode EndBeat;

	public ChordNode(ChordsNode chordsNode)
	{
		this.ChordsNode = chordsNode;
	}

	protected override void Parse()
	{
		StartBeat = ParseChild(new FloatExpressionNode());
		Parser.ConsumeToken(TokenType.Comma);
		EndBeat = ParseChild(new FloatExpressionNode());

		while (Parser.CursorToken.Type == TokenType.Identifier)
		{
			ParseChild(new NoteNode(this));
		}
	}

	protected override void Validate()
	{
		MelodyNode melodyNode = ChordsNode.MelodyNode;

		if (EndBeat.GetValue() > melodyNode.LengthInBeats)
		{
			Validator.AddError(this, $"Melody: {melodyNode.Id}. Note end time {EndBeat.GetValue()} exceeds melody length {melodyNode.LengthInBeats}");
		}
		if (StartBeat.GetValue() < 0 || EndBeat.GetValue() < 0)
		{
			Validator.AddError(this, $"Melody: {melodyNode.Id}. Start time and end time must be positive: {StartBeat.GetValue()}-{EndBeat.GetValue()}");
		}
		if (StartBeat.GetValue() >= EndBeat.GetValue())
		{
			Validator.AddError(this, $"Melody: {melodyNode.Id}. Start time must be less than end time: {StartBeat.GetValue()}-{EndBeat.GetValue()}");
		}
	}
}

