using System.Globalization;
using Ast.NodeArchetypes;
using Ast.Tables;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class PanNode : BranchNode
{
	public ModifiersNode ModifiersNode;
	public float Pan = 0;

	public PanNode(Node parent, ModifiersNode modifiersNode) : base(parent)
	{
		this.ModifiersNode = modifiersNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.PanKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => Pan = float.Parse(value, CultureInfo.InvariantCulture));
	}

	protected override void Validate(SemanticSymbolTable symbols)
	{
		NoteNode noteNode = ModifiersNode.NoteNode;
		MelodyNode melodyNode = noteNode.ChordNode.ChordsNode.MelodyNode;

		if (Pan < -1.0f || Pan > 1.0f)
		{
			Validator.AddError(this, $"Melody: '{melodyNode.Id}'. Note: '{noteNode.Pitch}'. Pan must be between -1 and 1, but was: {Pan}");
		}
	}

	protected override void Evaluate(RuntimeVariableTable variables)
	{
		Note note = ModifiersNode.NoteNode.Note;
		note.Pan = Pan;
	}
}

