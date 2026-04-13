using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes.Modifiers;

public class ModifiersNode : BranchNode
{
	public NoteNode NoteNode;

	public ModifiersNode(Node parent, NoteNode noteNode) : base(parent)
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
				() => { new GainNode(this, this); }
			},
			{
				TokenType.PanKeyword,
				() => { new PanNode(this, this); }
			}
		};
		Token[] optionSeparator = { new(TokenType.Comma) };
		Parser.HandleUniqueOptions(options, optionSeparator);

		Parser.ConsumeToken(TokenType.RightParentheses);
	}
}

