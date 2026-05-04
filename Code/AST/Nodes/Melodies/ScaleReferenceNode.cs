using Ast.Tables;
using Lexing.Tokens;
using Runtime.Objects;

namespace Ast.Nodes.Melodies;

public class ScaleReferenceNode(Node parent) : Node(parent)
{
	public string ScaleId = "";

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.ScaleKeyword);
		Parser.ConsumeToken(TokenType.Identifier,
			value => ScaleId = value);
	}

	protected override void Validate(NodeTable ancestors,
		SemanticSymbolTable symbols)
	{
		if (!symbols.Contains(typeof(ScaleNode), ScaleId))
		{
			Validator.AddError(this,
				$"Scale '{ScaleId}' is not declared");
		}
	}

	protected override void Evaluate(NodeTable ancestors,
	RuntimeVariableTable variables)
	{
		MelodyNode melodyNode =
			ancestors.Get<MelodyNode>();

		melodyNode.ScaleReferenceId = ScaleId;

		Scale scale =
			variables.Get<Scale>(ScaleId);

		melodyNode.Melody0.Scale = scale;
	}
}