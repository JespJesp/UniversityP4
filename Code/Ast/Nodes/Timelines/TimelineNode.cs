using Ast.NodeArchetypes;
using Ast.Nodes.Melodies;
using Phases.Parsing;
using Runtime;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Timelines;

public class TimelineNode : Node
{
	public static int TimelineInstances = 0;

	public TimelineNode()
	{
		TimelineInstances++;
	}

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.TimelineKeyword);

		// TODO: Implement this
	}

	public override void Validate()
	{
		if (TimelineInstances > 1)
		{
			throw new Exception("'timeline' keyword appears multiple times.");
		}

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

