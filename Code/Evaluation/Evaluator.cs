using AST;

namespace Evaluation;

public class Evaluator
{
	private static readonly Random _rng = new();

	public void Evaluate(Song song)
	{
		PrintToConsole(song);
	}

	private double EvaluateExpression(AST.IExpression expr)
	{
		switch (expr)
		{
			case AST.NumericExpression num:
				return num.Value;
			case AST.RandomExpression random:
				return EvaluateRandom(random);
			default:
				throw new Exception($"Unsupported expression type in evaluation: {expr?.GetType().Name}");
		}
	}

	private double EvaluateRandom(AST.RandomExpression expr)
	{
		double min = EvaluateExpression(expr.Min);
		double max = EvaluateExpression(expr.Max);

		if (min > max)
		{
			throw new Exception($"Runtime random() bounds invalid: min ({min}) cannot be greater than max ({max}).");
		}

		bool minIsInteger = Math.Abs(min % 1) < 1e-9;
		bool maxIsInteger = Math.Abs(max % 1) < 1e-9;

		if (minIsInteger && maxIsInteger)
		{
			int minInt = (int)Math.Ceiling(min);
			int maxInt = (int)Math.Floor(max);
			if (maxInt < minInt)
			{
				return min;
			}
			return _rng.Next(minInt, maxInt + 1);
		}

		return min + _rng.NextDouble() * (max - min);
	}

	// TODO: Delete this method. It's just an example
	private void PrintToConsole(Song song)
	{
		foreach (Pattern pattern in song.Patterns)
		{
			Console.WriteLine($"\n=== Pattern: {pattern.Name} ===");
			Console.WriteLine($"Length: {pattern.Length}");

			Console.WriteLine("Samples:");
			foreach (Sample sample in pattern.Samples)
			{
				Console.WriteLine($"  - {sample.FileName}");
			}

			Console.WriteLine("Notes:");
			foreach (Note note in pattern.Notes)
			{
				string pitchText;
				if (note.PitchExpression != null)
				{
					double pitchValue = EvaluateExpression(note.PitchExpression);
					if ((pitchValue % 1) == 0)
					{
						pitchText = ((int)pitchValue).ToString();
					}
					else
					{
						pitchText = pitchValue.ToString("0.###");
					}
				}
				else
				{
					pitchText = note.Pitch;
				}

				Console.WriteLine($"  - Time: {note.StartTime:D2}-{note.EndTime:D2}, Pitch: {pitchText}");
			}
		}
	}
}