using NAudio.Wave;

namespace Phases.Evaluation.AudioRendering.SampleProviders;

internal sealed class AdsrEnvelopeSampleProvider : ISampleProvider
{
	private readonly ISampleProvider _source;
	private readonly float _noteDurationSeconds;
	private readonly float _attackSeconds;
	private readonly float _holdSeconds;
	private readonly float _decaySeconds;
	private readonly float _sustainLevel;
	private readonly float _releaseSeconds;
	private readonly float _noteOffLevel;
	private long _framesRead;

	public WaveFormat WaveFormat => _source.WaveFormat;

	public AdsrEnvelopeSampleProvider(
			ISampleProvider source,
			float noteDurationSeconds,
			float attackSeconds,
			float holdSeconds,
			float decaySeconds,
			float sustainLevel,
			float releaseSeconds)
	{
		_source = source;
		_noteDurationSeconds = Math.Max(0.0f, noteDurationSeconds);
		_attackSeconds = Math.Max(0.0f, attackSeconds);
		_holdSeconds = Math.Max(0.0f, holdSeconds);
		_decaySeconds = Math.Max(0.0f, decaySeconds);
		_sustainLevel = Math.Clamp(sustainLevel, 0.0f, 1.0f);
		_releaseSeconds = Math.Max(0.0f, releaseSeconds);
		_noteOffLevel = GetLevelBeforeRelease(_noteDurationSeconds);
	}

	public int Read(float[] buffer, int offset, int count)
	{
		int samplesRead = _source.Read(buffer, offset, count);
		if (samplesRead == 0)
		{
			return 0;
		}

		int channels = WaveFormat.Channels;
		if (channels <= 0)
		{
			return samplesRead;
		}

		int framesRead = samplesRead / channels;
		for (int frame = 0; frame < framesRead; frame++)
		{
			float timeSeconds = (float)(_framesRead + frame) / WaveFormat.SampleRate;
			float envelopeGain = GetEnvelopeLevel(timeSeconds);
			int frameStart = offset + frame * channels;
			for (int channel = 0; channel < channels; channel++)
			{
				buffer[frameStart + channel] *= envelopeGain;
			}
		}

		_framesRead += framesRead;
		return samplesRead;
	}

	private float GetEnvelopeLevel(float timeSeconds)
	{
		if (timeSeconds < _noteDurationSeconds)
		{
			return GetLevelBeforeRelease(timeSeconds);
		}

		if (_releaseSeconds <= 0.0f)
		{
			return 0.0f;
		}

		float releaseElapsedSeconds = timeSeconds - _noteDurationSeconds;
		if (releaseElapsedSeconds >= _releaseSeconds)
		{
			return 0.0f;
		}

		float normalized = releaseElapsedSeconds / _releaseSeconds;
		return _noteOffLevel * (1.0f - normalized);
	}

	private float GetLevelBeforeRelease(float timeSeconds)
	{
		if (_attackSeconds > 0.0f && timeSeconds < _attackSeconds)
		{
			return timeSeconds / _attackSeconds;
		}

		float afterAttack = timeSeconds - _attackSeconds;
		if (_holdSeconds > 0.0f && afterAttack < _holdSeconds)
		{
			return 1.0f;
		}

		float afterHold = afterAttack - _holdSeconds;
		if (_decaySeconds > 0.0f && afterHold < _decaySeconds)
		{
			float normalized = afterHold / _decaySeconds;
			return 1.0f - ((1.0f - _sustainLevel) * normalized);
		}

		return _sustainLevel;
	}
}
