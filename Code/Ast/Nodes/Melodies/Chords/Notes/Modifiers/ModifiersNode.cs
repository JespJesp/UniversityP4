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

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.LeftParentheses);

		Dictionary<TokenType, Action> options = new()
		{
			{
				TokenType.GainKeyword,
				() => { Parser.ParseChild(this, new GainNode(this)); }
			},
			{
				TokenType.PanKeyword,
				() => { Parser.ParseChild(this, new PanNode(this)); }
			}
		};
		Token[] optionSeparator = { new(TokenType.Comma) };
		Parser.AllowUniqueOptions(options, optionSeparator);

		Parser.ConsumeToken(TokenType.RightParentheses);
	}
}

