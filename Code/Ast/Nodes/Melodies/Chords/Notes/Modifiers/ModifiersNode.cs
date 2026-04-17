using Ast.NodeArchetypes;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class ModifiersNode : Node
{
	public NoteNode NoteNode;

	public ModifiersNode(NoteNode noteNode)
	{
		this.NoteNode = noteNode;
	}

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.LeftParentheses);

		Dictionary<TokenType, Action> options = new()
		{
			{
				TokenType.GainKeyword,
				() => { parser.ParseChild(this, new GainNode(this)); }
			},
			{
				TokenType.PanKeyword,
				() => { parser.ParseChild(this, new PanNode(this)); }
			}
		};
		Token[] optionSeparator = { new(TokenType.Comma) };
		parser.AllowUniqueOptions(options, optionSeparator);

		parser.ConsumeToken(TokenType.RightParentheses);
	}
}

