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
		parser.ConsumeToken(TokenType.Identifier, out Id);
		_filePath = parser.ParseChild(this, new StringExpressionNode());
		parser.TryConsumeToken(TokenType.Identifier, out _referencePitch);
	}

	protected override void AdditionalValidation(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		if (!FilePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
			&& !FilePath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
			&& !FilePath.EndsWith(".aif", StringComparison.OrdinalIgnoreCase)
			&& !FilePath.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase))
		{
			Validator.AddError(this, $"Sample: '{Id}'. File path '{FilePath}' must be file of type .wav, .mp3, .aif, or .aiff");
		}
	}

	protected override void AdditionalEvaluation(NodeTable ancestors, RuntimeVariableTable variables)
	{
		Sample.FilePath = _filePath.Value;
		Sample.ReferencePitch = Pitch.FromString(_referencePitch);
	}
}

