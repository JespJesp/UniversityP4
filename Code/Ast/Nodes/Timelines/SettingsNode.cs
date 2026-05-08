using System.Globalization;
using Ast.Nodes.Floats;
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
	private FloatExpressionNode? _bpm;
	private FloatExpressionNode? _sampleRate;
	private FloatExpressionNode? _timeSignatureNumerator;
	private FloatExpressionNode? _timeSignatureDenominator;

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
							_bpm = parser.ParseChild(this, new FloatExpressionNode());
						}
					),
					(
						() => parser.TryConsumeToken(TokenType.Identifier, "samplerate"),
						() =>
						{
							_sampleRate = parser.ParseChild(this, new FloatExpressionNode());
						}
					),
					(
						() => parser.TryConsumeToken(TokenType.Identifier, "timesignature"),
						() =>
						{
							_timeSignatureNumerator = parser.ParseChild(this, new FloatExpressionNode());
							parser.ConsumeToken(TokenType.Comma);
							_timeSignatureDenominator = parser.ParseChild(this, new FloatExpressionNode());
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
		if (_bpm is not null && _bpm.Value <= 0)
		{
			errors.Add($"BPM '{_bpm}' must be positive");
		}
		if (_sampleRate is not null && _sampleRate.Value <= 0)
		{
			errors.Add($"Sample rate '{_sampleRate}' must be positive");
		}
		if (_timeSignatureNumerator is not null && _timeSignatureNumerator.Value <= 0
				&& _timeSignatureDenominator is not null && _timeSignatureDenominator.Value <= 0)
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
			timeline.SampleRate = (int)_sampleRate.Value;
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

