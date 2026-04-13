using Ast.NodeArchetypes;
using Ast.Tables;
using Runtime.Objects;
using Lexing.Tokens;
using Ast.Nodes.Primitives;

namespace Ast.Nodes.Samples;

public class SampleNode : VariableNode
{
	public StringExpressionNode FilePath;
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
		FilePath = new StringExpressionNode(this);
		Parser.TryConsumeToken(TokenType.Identifier, (value) => { ReferencePitch = value; });
	}

	protected override void AdditionalValidation(SemanticSymbolTable symbols)
	{
		string filePathValue = FilePath.GetValue();
		if (!filePathValue.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase))
		{
			Validator.AddError(this, $"Sample: '{Id}'. File path '{filePathValue}' must be file of type .wav, .mp3, .aif, or .aiff");
		}
	}

	protected override void AdditionalEvaluation(RuntimeVariableTable variables)
	{
		this.Sample.FilePath = this.FilePath.GetValue();
		this.Sample.ReferencePitch = new(this.ReferencePitch);
	}
}

