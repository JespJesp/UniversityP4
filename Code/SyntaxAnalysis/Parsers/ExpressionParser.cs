using System.Globalization;
using AST;
using LexicalAnalysis;

namespace SyntaxAnalysis.Parsers;

public static class ExpressionParser
{
	public static IExpression Parse(SyntaxAnalyzer a)
	{
		if (a.CursorToken().Type == TokenType.Integer || a.CursorToken().Type == TokenType.Float)
		{
			bool isInteger = a.CursorToken().Type == TokenType.Integer;
			double value = double.Parse(a.CursorToken().Value, CultureInfo.InvariantCulture);
			a.ConsumeToken(a.CursorToken().Type);
			return new NumericExpression(value, isInteger);
		}

		if (a.CursorToken().Type == TokenType.Identifier && a.CursorToken().Value == "random")
		{
			a.ConsumeToken(TokenType.Identifier);
			a.ConsumeToken(TokenType.LeftParen);
			IExpression minExpr = Parse(a);
			a.ConsumeToken(TokenType.Comma);
			IExpression maxExpr = Parse(a);
			a.ConsumeToken(TokenType.RightParen);
			return new RandomExpression(minExpr, maxExpr);
		}

		throw new Exception($"Unexpected expression token: {a.CursorToken()}");
	}
}