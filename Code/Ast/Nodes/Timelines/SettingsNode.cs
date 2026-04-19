using System.Globalization;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timeline;
using Tokens;

namespace Ast.Nodes.Timelines;

public class SettingsNode : Node
{
	public static int SettingsNodeInstances = 0;

	public TimelineNode timelineNode;
	// TODO: Maybe also add sample rate as optional parameter?
	public float? Bpm;
	public int? TimeSignatureNumerator;
	public int? TimeSignatureDenominator;

	public SettingsNode(TimelineNode timelineNode)
	{
		SettingsNodeInstances++;
		this.timelineNode = timelineNode;
	}

	public override void CascadeParse(Parser parser)
	{
		while (parser.TryConsumeIndent(2))
		{
			List<Func<bool>> options = new()
			{
				() => parser.TryConsumeToken(TokenType.Identifier, "bpm", (value) =>
						{
							// TODO: This could use a float expression node instead
							parser.ConsumeToken(TokenType.Float, value => Bpm = float.Parse(value, CultureInfo.InvariantCulture));
						}),
				() => parser.TryConsumeToken(TokenType.Identifier, "timesignature", (value) =>
						{
							parser.ConsumeToken(TokenType.Integer, value => TimeSignatureNumerator = int.Parse(value, CultureInfo.InvariantCulture));
							parser.ConsumeToken(TokenType.Slash);
							parser.ConsumeToken(TokenType.Integer, value => TimeSignatureDenominator = int.Parse(value, CultureInfo.InvariantCulture));
						}),
			};
			Token[] optionSeparator =
			{
				new(TokenType.Comma)
			};
			parser.TryConsumeOptions(options, optionSeparator);
		}
	}

	public override void Validate(Validator validator)
	{
		if (SettingsNodeInstances > 1)
		{
			throw new Exception("'settings' keyword appears multiple times in timeline.");
		}

		List<string> errors = new();
		if (Bpm is not null && Bpm <= 0)
		{
			errors.Add($"BPM '{Bpm}' must be positive.");
		}
		if (TimeSignatureNumerator is not null && TimeSignatureNumerator <= 0
				&& TimeSignatureDenominator is not null && TimeSignatureDenominator <= 0)
		{
			errors.Add($"Time signature values '{TimeSignatureNumerator}/{TimeSignatureDenominator}' must both be positive");
		}
		if (errors.Count != 0)
		{
			throw new Exception($"Timeline. Settings." + string.Join(" ", errors));
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Timeline timeline = timelineNode.Timeline;

		if (Bpm is not null)
		{
			timeline.BeatsPerMinute = Bpm.Value;
		}
		if (TimeSignatureNumerator is not null)
		{
			timeline.BeatsPerBar = TimeSignatureNumerator.Value;
		}
		if (TimeSignatureDenominator is not null)
		{
			timeline.BeatNoteValue = TimeSignatureDenominator.Value;
		}
	}
}

