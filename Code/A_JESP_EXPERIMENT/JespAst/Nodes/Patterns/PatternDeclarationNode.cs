using JespAst.Tables;
using JespRuntime.Nodes;
using LexicalAnalysis.Tokens;
using System.Globalization;

namespace JespAst.Nodes.Patterns;

public class PatternDeclarationNode(Node parent) : Node(parent)
{
	public string Id = "";
	public float LengthInBeats;
	public List<string> PatternAndMelodyIds = new();

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.PatternKeyword);
		Parser.ConsumeToken(TokenType.Float, (value) => { LengthInBeats = float.Parse(value, CultureInfo.InvariantCulture); });
		Parser.ConsumeToken(TokenType.Identifier, (value) => { Id = LengthInBeats + value; });

		while (Parser.TryConsumeIndent(1))
		{
			new ReferenceNode(this);
		}
	}

	protected override void Annotate(NodeTable localNodes, SymbolTable localSymbols)
	{
		localSymbols.Add(typeof(PatternDeclarationNode), Id);

		if (string.IsNullOrWhiteSpace(Id))
		{
			AddSemanticError("ID cannot be empty");
		}
		if (LengthInBeats <= 0)
		{
			AddSemanticError("Length cannot be <= 0");
		}
	}

	protected override void Evaluate(NodeTable localNodes, VariableTable localVariables)
	{
		Pattern pattern = new()
		{
			LengthInBeats = this.LengthInBeats
		};
		localVariables.Upsert(pattern, Id);
	}
}