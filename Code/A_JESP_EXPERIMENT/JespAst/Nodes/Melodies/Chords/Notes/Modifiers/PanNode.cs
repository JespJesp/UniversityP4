using System.Globalization;
using JespAst.Tables;
using JespRuntime.Objects;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Chords.Notes.Modifiers;

public class PanNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public float Pan = 0;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Float, (value) => Pan = float.Parse(value, CultureInfo.InvariantCulture));
	}

	protected override void Annotate(NodeTable ancestors, SymbolTable symbols)
	{
		MelodyNode melodyDeclarationNode = ancestors.Get<MelodyNode>();
		NoteNode noteNode = ancestors.Get<NoteNode>();

		if (Pan < -1.0f || Pan > 1.0f)
		{
			Annotator.AddSemanticError($"Melody: '{melodyDeclarationNode}'. Note: '{noteNode.Pitch}'. Pan must be between -1 and 1, but was: {Pan}");
		}
	}

	protected override void Evaluate(NodeTable ancestors, VariableTable variables)
	{
		Note note = ancestors.Get<NoteNode>().Note0;
		note.Pan = Pan;
	}
}

