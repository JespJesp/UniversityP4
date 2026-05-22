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
	private StringExpressionNode _filePath = new();
	private string _referencePitch = "";

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.Identifier, out Id);
		_filePath = parser.ParseChild(this, new StringExpressionNode());
		parser.TryConsumeToken(TokenType.Identifier, out _referencePitch);
	}

	public override void Validate(Validator validator)
	{
		string filePathValue = _filePath.Value;
		if (!filePathValue.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)
			&& !filePathValue.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase))
		{
			throw new Exception($"Sample: '{Id}'. File path '{filePathValue}' must be file of type .wav, .mp3, .aif, or .aiff");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Sample.FilePath = _filePath.Value;
		if (!string.IsNullOrEmpty(_referencePitch))
		{
			Sample.ReferencePitch = Pitch.FromString(_referencePitch);
		}
	}
}

