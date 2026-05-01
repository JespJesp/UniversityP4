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
		parser.ConsumeToken(TokenType.Float, out string lengthValue);
		LengthInBeats = float.Parse(lengthValue, CultureInfo.InvariantCulture);

		parser.ConsumeToken(TokenType.Identifier, out string nameValue);
		Id = LengthInBeats + nameValue;

		while (parser.TryConsumeIndent(1))
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
		Pattern.LengthInBeats = LengthInBeats;
	}
}