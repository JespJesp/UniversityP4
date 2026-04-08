using System.Globalization;
using JespAst.Tables;
using JespRuntime.Objects;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Chords.Notes.Modifiers;

public class GainNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public float Volume = 1;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Float, (value) => Volume = float.Parse(value, CultureInfo.InvariantCulture));
	}

	protected override void Annotate(NodeTable ancestors, SymbolTable symbols)
	{
		MelodyNode melodyDeclarationNode = ancestors.Get<MelodyNode>();
		NoteNode noteNode = ancestors.Get<NoteNode>();

		if (Volume < 0.0f)
		{
			Annotator.AddSemanticError($"Melody: '{melodyDeclarationNode}'. Note: '{noteNode.Pitch}'. Volume cannot be negative, but was: {Volume}");
		}
	}

	protected override void Evaluate(NodeTable ancestors, VariableTable variables)
	{
		Note note = ancestors.Get<NoteNode>().Note0;
		note.Volume = Volume;
	}
}

