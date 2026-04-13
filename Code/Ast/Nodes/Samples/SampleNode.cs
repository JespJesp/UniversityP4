using Ast.NodeArchetypes;
using Runtime.Objects;
using Lexing.Tokens;
using Ast.Nodes.Primitives;

namespace Ast.Nodes.Samples;

public class SampleNode : SymbolNode
{
	// TODO: public StringExpressionNode FilePath;
	public string FilePath;
	public string ReferencePitch = "";

	Sample Sample = new();
	protected override RuntimeObject GetRuntimeObject() => this.Sample;

	public SampleNode(Node parent) : base(parent)
	{
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SampleKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = value; });
		// TODO: FilePath = new StringExpressionNode(this);
		Parser.ConsumeToken(TokenType.String, (value) => { FilePath = value; });
		Parser.TryConsumeToken(TokenType.Identifier, (value) => { ReferencePitch = value; });
	}

	protected override void AdditionalAnnotation()
	{
		string filePathValue = FilePath; // TODO: FilePath.Value()
		if (!filePathValue.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase))
		{
			Annotator.AddError(this, $"Sample: '{Id}'. File path '{filePathValue}' must be file of type .wav, .mp3, .aif, or .aiff");
		}
	}

	protected override void Evaluate()
	{
		this.Sample.FilePath = this.FilePath; // TODO: this.FilePath.GetValue();
		this.Sample.ReferencePitch = new(this.ReferencePitch);
	}
}

