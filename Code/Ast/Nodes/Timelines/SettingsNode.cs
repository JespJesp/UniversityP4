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
	private float? _bpm;
	private int? _sampleRate;
	private int? _timeSignatureNumerator;
	private int? _timeSignatureDenominator;

	public SettingsNode(TimelineNode timelineNode)
	{
		this.timelineNode = timelineNode;
	}

	public override void CascadeParse(Parser parser)
	{
		SettingsNodeInstances++;

		if (parser.TryConsumeNewlineIndent(2))
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
							_bpm = float.Parse(bpmValue, CultureInfo.InvariantCulture);
						}
					),
					(
						() => parser.TryConsumeToken(TokenType.Identifier, "samplerate"),
						() =>
						{
							parser.ConsumeToken(TokenType.Integer, out string sampleRateValue);
							_sampleRate = int.Parse(sampleRateValue);
						}
					),
					(
						() => parser.TryConsumeToken(TokenType.Identifier, "timesignature"),
						() =>
						{
							parser.ConsumeToken(TokenType.Integer, out string numeratorValue);
							_timeSignatureNumerator = int.Parse(numeratorValue, CultureInfo.InvariantCulture);

							parser.ConsumeToken(TokenType.Slash);

							parser.ConsumeToken(TokenType.Integer, out string denominatorValue);
							_timeSignatureDenominator = int.Parse(denominatorValue, CultureInfo.InvariantCulture);
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
		if (_bpm is not null && _bpm <= 0)
		{
			errors.Add($"BPM '{_bpm}' must be positive");
		}
		if (_bpm is not null && _sampleRate <= 0)
		{
			errors.Add($"Sample rate '{_bpm}' must be positive");
		}
		if (_timeSignatureNumerator is not null && _timeSignatureNumerator <= 0
				&& _timeSignatureDenominator is not null && _timeSignatureDenominator <= 0)
		{
			errors.Add($"Time signature values '{_timeSignatureNumerator}/{_timeSignatureDenominator}' must both be positive");
		}
		if (errors.Count != 0)
		{
			throw new Exception($"Timeline. Settings." + string.Join(" ", errors));
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Timeline timeline = timelineNode.Timeline;

		if (_bpm is not null)
		{
			timeline.BeatsPerMinute = _bpm.Value;
		}
		if (_sampleRate is not null)
		{
			timeline.SampleRate = _sampleRate.Value;
		}
		if (_timeSignatureNumerator is not null)
		{
			timeline.BeatsPerBar = _timeSignatureNumerator.Value;
		}
		if (_timeSignatureDenominator is not null)
		{
			timeline.BeatNoteValue = _timeSignatureDenominator.Value;
		}
	}
}

