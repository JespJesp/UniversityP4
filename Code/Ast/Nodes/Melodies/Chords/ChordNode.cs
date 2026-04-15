using Ast.NodeArchetypes;
using Ast.Nodes.Floats;
using Ast.Nodes.Melodies.Chords.Notes;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords;

public class ChordNode : BranchNode
{
	public ChordsNode ChordsNode;
	public FloatExpressionNode StartBeat = new();
	public FloatExpressionNode EndBeat = new();

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

		List<string> errors = new();
		if (EndBeat.Value > melodyNode.LengthInBeats)
		{
			errors.Add($"Note end time '{EndBeat.Value}' exceeds melody length '{melodyNode.LengthInBeats}'.");
		}
		if (StartBeat.Value < 0 || EndBeat.Value < 0)
		{
			errors.Add($"Start time and end time must be positive: '{StartBeat.Value},{EndBeat.Value}'.");
		}
		if (StartBeat.Value >= EndBeat.Value)
		{
			errors.Add($"Start time must be less than end time: '{StartBeat.Value},{EndBeat.Value}'.");
		}
		if (errors.Count != 0)
		{
			throw new Exception($"Melody: {melodyNode.Id}." + string.Join(" ", errors));
		}
	}
}

