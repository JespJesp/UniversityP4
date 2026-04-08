using JespAst.Tables;
using JespRuntime.Objects;
using LexicalAnalysis.Tokens;
using System.Globalization;

namespace JespAst.Nodes.Patterns;

public class PatternNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";
	public float LengthInBeats;
	public List<string> PatternAndMelodyIds = new();
	public Pattern Pattern0;

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

	protected override void Annotate(NodeTable ancestors, SymbolTable symbols)
	{
		symbols.Add(typeof(PatternNode), Id);

		if (LengthInBeats <= 0)
		{
			Annotator.AddSemanticError($"Pattern: '{Id}'. Length cannot be <= 0");
		}
	}

	protected override void Evaluate(NodeTable ancestors, VariableTable localVariables)
	{
		this.Pattern0 = new()
		{
			LengthInBeats = this.LengthInBeats
		};
		localVariables.Upsert(this.Pattern0, Id);
	}
}