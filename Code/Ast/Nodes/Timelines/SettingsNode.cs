using System.Globalization;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timelines;
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
		this.timelineNode = timelineNode;
	}

	public override void CascadeParse(Parser parser)
	{
		SettingsNodeInstances++;

		if (parser.TryConsumeIndent(2))
		{
			parser.TryConsumeOptions
			(
				new()
				{
					(
						() => parser.TryConsumeToken(TokenType.Identifier, "bpm"),
						() =>
						{
							parser.ConsumeToken(TokenType.Float, out string bpmValue);
							Bpm = float.Parse(bpmValue, CultureInfo.InvariantCulture);
						}
					),
					(
						() => parser.TryConsumeToken(TokenType.Identifier, "timesignature"),
						() =>
						{
							parser.ConsumeToken(TokenType.Integer, out string numeratorValue);
							TimeSignatureNumerator = int.Parse(numeratorValue, CultureInfo.InvariantCulture);

							parser.ConsumeToken(TokenType.Slash);

							parser.ConsumeToken(TokenType.Integer, out string denominatorValue);
							TimeSignatureDenominator = int.Parse(denominatorValue, CultureInfo.InvariantCulture);
						}
					),
				},
				[
					new(TokenType.Newline),
					new(TokenType.Indent, "2"),
				]
			);
		}
	}

	public override void Validate(Validator validator)
	{
		if (SettingsNodeInstances > 1)
		{
			throw new Exception("'settings' keyword appears multiple times in timeline");
		}

		List<string> errors = new();
		if (Bpm is not null && Bpm <= 0)
		{
			errors.Add($"BPM '{Bpm}' must be positive");
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

