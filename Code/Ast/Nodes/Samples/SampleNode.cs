using Ast.NodeArchetypes;
using Runtime.Objects;
using Lexing.Tokens;
using Ast.Nodes.Strings;

namespace Ast.Nodes.Samples;

public class SampleNode : SymbolNode
{
	public StringExpressionNode FilePath;
	public string ReferencePitch = "";

	public Sample Sample = new();

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SampleKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = value; });
		FilePath = ParseChild(new StringExpressionNode());
		Parser.TryConsumeToken(TokenType.Identifier, (value) => { ReferencePitch = value; });
	}

	protected override void Validate()
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

	protected override void Evaluate()
	{
		this.Sample.FilePath = this.FilePath.GetValue();
		this.Sample.ReferencePitch = new(this.ReferencePitch);
	}
}

