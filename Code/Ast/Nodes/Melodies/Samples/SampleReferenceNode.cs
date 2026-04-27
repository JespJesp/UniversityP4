using Ast.Tables;
using Ast.Nodes.Samples;
using Runtime.Objects;
using Lexing.Tokens;
using System.Globalization;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferenceNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";
	public float DelayBeats;
	public float AttackBeats;
	public float HoldBeats;
	public float DecayBeats;
	public float SustainLevel = 1.0f;
	public float ReleaseBeats;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Identifier, (value) => Id = value);

		if (!Parser.TryConsumeToken(TokenType.LeftParentheses))
		{
			return;
		}

		HashSet<string> usedModifiers = new(StringComparer.OrdinalIgnoreCase);
		while (true)
		{
			string modifierName = "";
			Parser.ConsumeToken(TokenType.Identifier, value => modifierName = value);

			if (!usedModifiers.Add(modifierName))
			{
				throw new Exception($"Duplicate sample modifier '{modifierName}'");
			}

			switch (modifierName.ToLowerInvariant())
			{
				case "delay":
					Parser.ConsumeToken(TokenType.Float, value => DelayBeats = float.Parse(value, CultureInfo.InvariantCulture));
					break;
				case "attack":
					Parser.ConsumeToken(TokenType.Float, value => AttackBeats = float.Parse(value, CultureInfo.InvariantCulture));
					break;
				case "hold":
					Parser.ConsumeToken(TokenType.Float, value => HoldBeats = float.Parse(value, CultureInfo.InvariantCulture));
					break;
				case "decay":
					Parser.ConsumeToken(TokenType.Float, value => DecayBeats = float.Parse(value, CultureInfo.InvariantCulture));
					break;
				case "sustain":
					Parser.ConsumeToken(TokenType.Float, value => SustainLevel = float.Parse(value, CultureInfo.InvariantCulture));
					break;
				case "release":
					Parser.ConsumeToken(TokenType.Float, value => ReleaseBeats = float.Parse(value, CultureInfo.InvariantCulture));
					break;
				default:
					throw new Exception($"Unknown sample modifier: {modifierName}");
			}

			if (!Parser.TryConsumeToken(TokenType.Comma))
			{
				break;
			}
		}

		Parser.ConsumeToken(TokenType.RightParentheses);
	}

	protected override void Validate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		MelodyNode melodyNode = ancestors.Get<MelodyNode>();

		if (!symbols.Contains(typeof(SampleNode), Id))
		{
			Validator.AddError(this, $"Melody: '{melodyNode.Id}'. The sample reference '{Id}' is not declared");
		}

		if (DelayBeats < 0 || AttackBeats < 0 || HoldBeats < 0 || DecayBeats < 0 || ReleaseBeats < 0)
		{
			Validator.AddError(this, $"Melody: '{melodyNode.Id}'. Sample reference '{Id}' has negative envelope timing values");
		}

		if (SustainLevel < 0.0f || SustainLevel > 1.0f)
		{
			Validator.AddError(this, $"Melody: '{melodyNode.Id}'. Sample reference '{Id}' sustain must be between 0 and 1");
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		Melody melody = variables.Get<Melody>(ancestors.Get<MelodyNode>().Id);
		Sample sourceSample = variables.Get<Sample>(Id);
		Sample configuredSample = new()
		{
			FilePath = sourceSample.FilePath,
			ReferencePitch = sourceSample.ReferencePitch,
			DelayBeats = DelayBeats,
			AttackBeats = AttackBeats,
			HoldBeats = HoldBeats,
			DecayBeats = DecayBeats,
			SustainLevel = SustainLevel,
			ReleaseBeats = ReleaseBeats
		};

		melody.Samples.Add(configuredSample);
	}
}

