using JespAst.Tables;
using JespRuntime.Nodes;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Samples;

public class SampleDeclaration(Node parent) : Node(parent)
{
	public string Id = "";
	public string FilePath = "";
	public string ReferencePitch = "";

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = value; });
		Parser.ConsumeToken(TokenType.String, (value) => { FilePath = value; });
		Parser.TryConsumeToken(TokenType.Identifier, (value) => { ReferencePitch = value; });
	}

	protected override void Annotate(NodeTable localNodes, SymbolTable localSymbols)
	{
		localSymbols.Add(typeof(SampleDeclaration), Id);

		if (string.IsNullOrWhiteSpace(Id))
		{
			AddSemanticError("ID cannot be empty");
		}
		if (string.IsNullOrWhiteSpace(FilePath))
		{
			AddSemanticError($"Sample '{Id}' file path name cannot be empty");
		}
		if (!FilePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
		{
			AddSemanticError($"Sample '{Id}' with file path '{FilePath}' must be a .wav file");
			// TODO: Also allow for .mp3, .flac, and such (all the audio files that NAudio supports)
		}
	}

	protected override void Evaluate(NodeTable localNodes, VariableTable localVariables)
	{
		Sample sample = new()
		{
			FilePath = this.FilePath,
			ReferencePitch = new(this.ReferencePitch)
		};
		localVariables.Upsert(sample, Id);
	}
}

