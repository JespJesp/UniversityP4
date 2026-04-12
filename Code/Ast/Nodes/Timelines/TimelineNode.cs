using Ast.Tables;
using Lexing.Tokens;
using Runtime;
using Runtime.Objects;

namespace Ast.Nodes.Timelines;

public class TimelineNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	protected override void Parse()
	{
		// TODO: Implement this
		Parser.ConsumeToken(TokenType.TimelineKeyword);
	}

	protected override void Validate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		// TODO: Implement this
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		// TODO: Implement this

		// TODO: Remove this; it's for testing
		Loop exampleLoop1 = new()
		{
			Melody0 = variables.Get<Melody>("8_guitar"),
			StartBeat = 0,
			EndBeat = 8
		};
		Loop exampleLoop2 = new()
		{
			Melody0 = variables.Get<Melody>("16_flute"),
			StartBeat = 12,
			EndBeat = 64
		};
		Timeline.Loops.Add(exampleLoop1);
		Timeline.Loops.Add(exampleLoop2);
	}
}

