using System.Globalization;
using Ast.Tables;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class PanNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public float Pan = 0;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.PanKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => Pan = float.Parse(value, CultureInfo.InvariantCulture));
	}

	protected override void Validate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		MelodyNode melodyNode = ancestors.Get<MelodyNode>();
		NoteNode noteNode = ancestors.Get<NoteNode>();

		if (Pan < -1.0f || Pan > 1.0f)
		{
			Validator.AddError(this, $"Melody: '{melodyNode.Id}'. Note: '{noteNode.Pitch}'. Pan must be between -1 and 1, but was: {Pan}");
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		Note note = ancestors.Get<NoteNode>().Note0;
		note.Pan = Pan;
	}
}

