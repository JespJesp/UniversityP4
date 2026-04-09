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

	protected override void Annotate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		// TODO: Implement this
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		// TODO: Implement this

		// TODO: Remove this; it's for testing
		Timeline.Loops.Add(new(variables.Get<Melody>("8_guitar"), 0, 8));
		Timeline.Loops.Add(new(variables.Get<Melody>("16_flute"), 12, 64));
	}
}

