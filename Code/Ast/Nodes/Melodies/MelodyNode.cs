using System.Globalization;
using Ast.Tables;
using Runtime.Objects;
using Ast.Nodes.Melodies.Chords;
using Ast.Nodes.Melodies.Samples;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies;

public class MelodyNode(Node parent, bool createsNestedScope = false) : VariableNode(parent, createsNestedScope)
{
	public float LengthInBeats;
	public Melody Melody0 = new();

	public string? ScaleReferenceId = null;

	protected override void Parse()
	{
		parser.ConsumeToken(TokenType.Float, out string lengthValue);
		LengthInBeats = float.Parse(lengthValue, CultureInfo.InvariantCulture);

		parser.ConsumeToken(TokenType.Identifier, out string nameValue);
		Id = LengthInBeats + nameValue;

		if (parser.TryConsumeNewlineIndent(1))
		{
			{
				TokenType.ScaleKeyword,
				() => { new ScaleReferenceNode(this); }
			},
			{
				TokenType.SamplesKeyword,
				() => { new SampleReferencesNode(this); }
			},
			{
				TokenType.ChordsKeyword,
				() => { new ChordsNode(this); }
			}
		};
		Token[] optionSeparator = { new(TokenType.Newline), new(TokenType.Indent, "1") };
		Parser.HandleUniqueOptions(options, optionSeparator);
	}

	protected override void AdditionalValidation(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		if (LengthInBeats <= 0)
		{
			Validator.AddError(this, $"Melody: '{Id}'. Length cannot be <= 0");
		}
	}

	protected override void AdditionalEvaluation(
	NodeTable ancestors,
	RuntimeVariableTable variables)
{
	this.Melody0.LengthInBeats =
		this.LengthInBeats;

	if (ScaleReferenceId != null)
	{
		if (variables.TryGet(
			ScaleReferenceId,
			out Scale scale))
		{
			this.Melody0.Scale = scale;
		}
		else
		{
			throw new Exception(
				$"Scale '{ScaleReferenceId}' not found at runtime");
		}
	}
}

	protected override RuntimeObject GetRuntimeObject()
	{
		return this.Melody0;
		Melody.LengthInBeats = LengthInBeats;
	}
}

