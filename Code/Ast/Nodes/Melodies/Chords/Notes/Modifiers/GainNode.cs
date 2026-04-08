using System.Globalization;
using Ast.Tables;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class GainNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public float Volume = 1;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.GainKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => Volume = float.Parse(value, CultureInfo.InvariantCulture));
	}

	protected override void Annotate(NodeTable ancestors, SymbolTable symbols)
	{
		MelodyNode melodyNode = ancestors.Get<MelodyNode>();
		NoteNode noteNode = ancestors.Get<NoteNode>();

		if (Volume < 0.0f)
		{
			Annotator.AddSemanticError(this, $"Melody: '{melodyNode}'. Note: '{noteNode.Pitch}'. Volume cannot be negative, but was: {Volume}");
		}
	}

	protected override void Evaluate(NodeTable ancestors, VariableTable variables)
	{
		Note note = ancestors.Get<NoteNode>().Note0;
		note.Volume = Volume;
	}
}

