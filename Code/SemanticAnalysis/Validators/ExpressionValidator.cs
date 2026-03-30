using AST;

namespace SemanticAnalysis.Validators;

public static class ExpressionValidator
{
	public static void Validate(SemanticAnalyzer analyzer, IExpression expression)
	{
		switch (expression)
		{
			case NumericExpression numeric:
				// numeric values are always valid
				return;
			case RandomExpression random:
				Validate(analyzer, random.Min);
				Validate(analyzer, random.Max);

				if (!IsNumeric(random.Min) || !IsNumeric(random.Max))
				{
					analyzer.AddError("Random() arguments must be numeric expressions.");
					return;
				}

				double? min = TryEvaluateAsDouble(random.Min);
				double? max = TryEvaluateAsDouble(random.Max);

				if (min.HasValue && max.HasValue && min.Value > max.Value)
				{
					analyzer.AddError($"Random() min must be <= max. got min={min.Value}, max={max.Value}.");
				}
				return;
			default:
				analyzer.AddError($"Unknown expression type: {expression?.GetType().Name}");
				return;
		}
	}

	public static bool IsNumeric(IExpression expression)
	{
		return expression is NumericExpression || expression is RandomExpression;
	}

	public static double? TryEvaluateAsDouble(IExpression expression)
	{
		switch (expression)
		{
			case NumericExpression numeric:
				return numeric.Value;
			case RandomExpression random:
				// Cannot determine runtime specific value at compile-time for nested random functions.
				return null;
			default:
				return null;
		}
	}

	public static double EvaluateAsDouble(IExpression expression)
	{
		switch (expression)
		{
			case NumericExpression numeric:
				return numeric.Value;
			case RandomExpression random:
				throw new Exception("Random expression cannot be evaluated to a deterministic value in semantic phase.");
			default:
				throw new Exception($"Cannot evaluate expression type: {expression?.GetType().Name}");
		}
	}
}

