using Ast.Nodes.Strings;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Samples;

public class SampleNode : SymbolNode
{
	public Sample Sample = new();
	public StringExpressionNode FilePath = new();
	public string ReferencePitch = "";

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.SampleKeyword);
		parser.ConsumeToken(TokenType.Identifier, (value) => { Id = value; });
		FilePath = parser.ParseChild(this, new StringExpressionNode());
		parser.TryConsumeToken(TokenType.Identifier, (value) => { ReferencePitch = value; });
	}

	public override void Validate(Validator validator)
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

	public override void Evaluate(Evaluator evaluator)
	{
		this.Sample.FilePath = this.FilePath.Value;
		this.Sample.ReferencePitch = Pitch.FromString(this.ReferencePitch);
	}
}

