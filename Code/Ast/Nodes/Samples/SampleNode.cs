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
		if (!FilePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
			&& !FilePath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
			&& !FilePath.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)
			&& !FilePath.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase))
		{
			Annotator.AddError(this, $"Sample: '{Id}'. File path '{FilePath}' must be file of type .wav, .mp3, .aif, or .aiff");
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

