using Ast.Nodes.Floats;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;
using Ast.Nodes.Samples;
using Ast.Nodes.Strings;
using Ast.Nodes.Timelines;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes;

public class FileNode : Node
{
	public override void CascadeParse(Parser parser)
	{
		while (!parser.AtEndOfTokens)
		{
			if (parser.TryConsumeToken(TokenType.Identifier, "timeline"))
			{
				parser.ParseChild(this, new TimelineNode());
			}
			else if (parser.TryConsumeToken(TokenType.Identifier, "pattern"))
			{
				parser.ParseChild(this, new PatternNode());
			}
			else if (parser.TryConsumeToken(TokenType.Identifier, "melody"))
			{
				parser.ParseChild(this, new MelodyNode());
			}
			else if (parser.TryConsumeToken(TokenType.Identifier, "sample"))
			{
				parser.ParseChild(this, new SampleNode());
			}
			else if (parser.TryConsumeToken(TokenType.Identifier, "string"))
			{
				parser.ParseChild(this, new StringConstantNode());
			}
			else if (parser.TryConsumeToken(TokenType.Identifier, "float"))
			{
				parser.ParseChild(this, new FloatConstantNode());
			}
			else if (parser.TryConsumeToken(TokenType.Newline))
			{
				// Do nothing
			}
			else if (parser.TryConsumeToken(TokenType.EndOfImportedFile))
			{
				parser.ParseChild(this, new FileNode(), createsNestedScope: true);
			}
			else
			{
				parser.AddErrorAndSkipLine(this, $"Unexpected program instruction");
			}
		}
	}
}

