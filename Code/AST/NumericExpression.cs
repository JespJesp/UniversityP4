namespace AST
{
    public class NumericExpression : IExpression
    {
        public double Value { get; }
        public bool IsInteger { get; }

        public NumericExpression(double value, bool isInteger)
        {
            Value = value;
            IsInteger = isInteger;
        }
    }
}
