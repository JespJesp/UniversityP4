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
		List<Func<bool>> options = new()
			{
				() => parser.TryConsumeToken(TokenType.Identifier, "gain", (value) => parser.ParseChild(this, new GainNode(this))),
				() => parser.TryConsumeToken(TokenType.Identifier, "pan", (value) => parser.ParseChild(this, new PanNode(this))),
			};
		Token[] optionSeparator =
		{
			new(TokenType.Comma)
		};
		parser.TryConsumeOptions(options, optionSeparator);

		parser.ConsumeToken(TokenType.RightParentheses);
	}
}

