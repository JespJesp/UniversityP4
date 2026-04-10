using Ast.Tables;
using Runtime.Objects;
using Lexing.Tokens;

namespace Ast.Nodes.Samples;

public class SampleNode(Node parent, bool createsNestedScope = false) : VariableNode(parent, createsNestedScope)
{
	public string FilePath = "";
	public string ReferencePitch = "";
	Sample Sample0 = new();

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SampleKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = value; });
		Parser.ConsumeToken(TokenType.String, (value) => { FilePath = value; });
		Parser.TryConsumeToken(TokenType.Identifier, (value) => { ReferencePitch = value; });
	}

	protected override void AdditionalAnnotation(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		if (!FilePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
		{
			Annotator.AddSemanticError(this, $"Sample: '{Id}'. File path '{FilePath}' must be a .wav file");
			// TODO: Also allow for .mp3, .flac, and such (all the audio files that NAudio supports)
		}
	}

	protected override void AdditionalEvaluation(NodeTable ancestors, RuntimeVariableTable variables)
	{
		this.Sample0.FilePath = this.FilePath;
		this.Sample0.ReferencePitch = new(this.ReferencePitch);
	}

	protected override RuntimeObject GetRuntimeObject()
	{
		return this.Sample0;
	}
}

