using Ast.Nodes.Melodies.Chords.Notes.Modifiers;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes;

public class ModifiersNode : Node
{
	public NoteNode NoteNode;

	public ModifiersNode(NoteNode noteNode)
	{
		this.NoteNode = noteNode;
	}

	public override void CascadeParse(Parser parser)
	{
		parser.TryConsumeOptions
		(
			new()
			{
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "gain"),
					() => parser.ParseChild(this, new GainNode(this))
				),
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "pan"),
					() => parser.ParseChild(this, new PanNode(this))
				),
			},
			[
				new(TokenType.Comma)
			]
		);

		parser.ConsumeToken(TokenType.RightParentheses);
	}
}

