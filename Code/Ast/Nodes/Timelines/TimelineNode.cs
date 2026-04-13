using Ast.NodeArchetypes;
using Lexing.Tokens;
using Runtime;
using Runtime.Objects;

namespace Ast.Nodes.Timelines;

public class TimelineNode : BranchNode
{
	public TimelineNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		// TODO: Implement this
		Parser.ConsumeToken(TokenType.TimelineKeyword);
	}

	protected override void Annotate()
	{
		// TODO: Implement this
	}

	protected override void Evaluate()
	{
		// TODO: Implement this

		// TODO: Remove this; it's for testing
		Loop exampleLoop1 = new()
		{
			Melody = _symbolTable.Get<Melody>("8_guitar"),
			StartBeat = 0,
			EndBeat = 8
		};
		Loop exampleLoop2 = new()
		{
			Melody = _symbolTable.Get<Melody>("16_flute"),
			StartBeat = 12,
			EndBeat = 64
		};
		Timeline.Loops.Add(exampleLoop1);
		Timeline.Loops.Add(exampleLoop2);
	}
}

