using Ast.NodeArchetypes;
using Parsing;
using Ast.Nodes.Melodies;
using Lexing.Tokens;
using Runtime;
using Runtime.Objects;

namespace Ast.Nodes.Timelines;

public class TimelineNode : BranchNode
{
	protected override void Parse()
	{
		// TODO: Implement this
		Parser.ConsumeToken(TokenType.TimelineKeyword);
	}

	protected override void Validate()
	{
		// TODO: Implement this
	}

	protected override void Evaluate()
	{
		// TODO: Implement this

		// TODO: Remove this; it's for testing
		Loop exampleLoop1 = new()
		{
			Melody = _symbolTable.Get<MelodyNode>("8_guitar").Melody,
			StartBeat = 0,
			EndBeat = 8
		};
		Loop exampleLoop2 = new()
		{
			Melody = _symbolTable.Get<MelodyNode>("16_flute").Melody,
			StartBeat = 12,
			EndBeat = 64
		};
		Timeline.Loops.Add(exampleLoop1);
		Timeline.Loops.Add(exampleLoop2);
	}
}

