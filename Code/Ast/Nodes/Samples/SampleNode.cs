using Ast.Tables;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Samples;

public class SampleNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";
	public string FilePath = "";
	public string ReferencePitch = "";
	Sample Sample0;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SampleKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = value; });
		Parser.ConsumeToken(TokenType.String, (value) => { FilePath = value; });
		Parser.TryConsumeToken(TokenType.Identifier, (value) => { ReferencePitch = value; });
	}

	protected override void Annotate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		symbols.Add(this, Id);

		if (!FilePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
		{
			Annotator.AddSemanticError(this, $"Sample: '{Id}'. File path '{FilePath}' must be a .wav file");
			// TODO: Also allow for .mp3, .flac, and such (all the audio files that NAudio supports)
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		this.Sample0 = new()
		{
			FilePath = this.FilePath,
			ReferencePitch = new(this.ReferencePitch)
		};
		variables.Upsert(this.Sample0, Id);
	}
}

