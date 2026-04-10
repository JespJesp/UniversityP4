using NAudio.Wave;
 
namespace Ast;
public class StereoPanningSampleProvider : ISampleProvider
	{
		private readonly ISampleProvider _source;
		private float _pan;
 
		public float Pan
		{
			get => _pan;
			set => _pan = Math.Clamp(value, -1f, 1f);
		}
 
		public WaveFormat WaveFormat => _source.WaveFormat;
 
		public StereoPanningSampleProvider(ISampleProvider source, float pan = 0f)
		{
			if (source.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
				throw new ArgumentException("Source must provide IEEE float samples.", nameof(source));
 
			if (source.WaveFormat.Channels != 2)
				throw new ArgumentException(
					"StereoPanningSampleProvider requires a stereo source.", nameof(source));
 
			_source = source;
			Pan = pan;
		}
 
		public int Read(float[] buffer, int offset, int count)
		{
			int samplesRead = _source.Read(buffer, offset, count);
 
			// Snapshot pan so it cannot change mid-buffer if another thread writes Pan.
			float pan = _pan;
 
			// Constant-power (equal-power) panning.
			// Maps pan ∈ [-1, 1] → angle ∈ [0, π/2].
			// cos²θ + sin²θ = 1, so total power is preserved at every pan position.
			// A linear balance law produces a ~3 dB dip at centre by comparison.
			// AI har lavet matematikken, jeg fatter ikke en bjælde af det men det bør checke ud med hvad DAW's har
			float angle     = (pan + 1f) * MathF.PI / 4f;
			float leftGain  = MathF.Cos(angle);
			float rightGain = MathF.Sin(angle);


			int end = offset + samplesRead;
			for (int i = offset; i < end; i += 2)
			{
				buffer[i] *= leftGain;
				if (i + 1 < end)
					buffer[i + 1] *= rightGain;
			}
 
			return samplesRead;
		}
	}