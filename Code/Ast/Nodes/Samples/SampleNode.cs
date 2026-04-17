using Ast.NodeArchetypes;
using Ast.Nodes.Strings;
using Phases.Parsing;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Samples;

public class SampleNode : SymbolNode
{
	public StringExpressionNode FilePath = new();
	public string ReferencePitch = "";
	public Sample Sample = new();

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.SampleKeyword);
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = value; });
		FilePath = Parser.ParseChild(this, new StringExpressionNode());
		Parser.TryConsumeToken(TokenType.Identifier, (value) => { ReferencePitch = value; });
	}

	public override void Validate()
	{
		string filePathValue = FilePath.Value;
		if (!filePathValue.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase))
		{
			throw new Exception($"Sample: '{Id}'. File path '{filePathValue}' must be file of type .wav, .mp3, .aif, or .aiff.");
		}
	}

	public override void Evaluate()
	{
		this.Sample.FilePath = this.FilePath.Value;
		this.Sample.ReferencePitch = Pitch.FromString(this.ReferencePitch);
	}
}

