
namespace AST
{
    public class RandomExpression : IExpression
    {
        public IExpression Min { get; }
        public IExpression Max { get; }

        public RandomExpression(IExpression min, IExpression max)
        {
            Min = min ?? throw new ArgumentNullException(nameof(min));
            Max = max ?? throw new ArgumentNullException(nameof(max));
        }
    }
}
