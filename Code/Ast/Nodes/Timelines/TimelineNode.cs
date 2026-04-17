using Ast.NodeArchetypes;
using Ast.Nodes.Melodies;
using Phases.Parsing;
using Runtime;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Timelines;

public class TimelineNode : Node
{
	public override void CascadeParse()
	{
		// TODO: Implement this
		Parser.ConsumeToken(TokenType.TimelineKeyword);
	}

	public override void Validate()
	{
		// TODO: Implement this
	}

	public override void Evaluate()
	{
		// TODO: Implement this

		// TODO: Remove this; it's for testing
		Loop exampleLoop1 = new()
		{
			Melody = SymbolTable.Get<MelodyNode>("8_guitar").Melody,
			StartBeat = 0,
			EndBeat = 8
		};
		Loop exampleLoop2 = new()
		{
			Melody = SymbolTable.Get<MelodyNode>("16_flute").Melody,
			StartBeat = 12,
			EndBeat = 64
		};
		Timeline.Loops.Add(exampleLoop1);
		Timeline.Loops.Add(exampleLoop2);
	}
}

