using Ast.NodeArchetypes;
using Parsing;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class ModifiersNode : BranchNode
{
	public NoteNode NoteNode;

	public ModifiersNode(NoteNode noteNode)
	{
		this.NoteNode = noteNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.LeftParentheses);

		Dictionary<TokenType, Action> options = new()
		{
			{
				TokenType.GainKeyword,
				() => { ParseChild(new GainNode(this)); }
			},
			{
				TokenType.PanKeyword,
				() => { ParseChild(new PanNode(this)); }
			}
		};
		Token[] optionSeparator = { new(TokenType.Comma) };
		Parser.AllowUniqueOptions(options, optionSeparator);

		Parser.ConsumeToken(TokenType.RightParentheses);
	}
}

