using Ast.Tables;
using Runtime.Objects;
using Lexing.Tokens;
using System.Globalization;

namespace Ast.Nodes.Patterns;

public class PatternNode(Node parent, bool createsNestedScope = false) : VariableNode(parent, createsNestedScope)
{
	public float LengthInBeats;
	public List<string> PatternAndMelodyIds = new();
	public Pattern Pattern0 = new();

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

	protected override void AdditionalValidation(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		if (LengthInBeats <= 0)
		{
			Validator.AddError(this, $"Pattern: '{Id}'. Length cannot be <= 0");
		}
	}

	protected override void AdditionalEvaluation(NodeTable ancestors, RuntimeVariableTable localVariables)
	{
		this.Pattern0.LengthInBeats = this.LengthInBeats;
	}

	protected override RuntimeObject GetRuntimeObject()
	{
		return this.Pattern0;
	}
}