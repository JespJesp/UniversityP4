namespace Tokens;

public static class TokenTypeExtensions
{
	public static bool IsSubtypeOf(this TokenType type, TokenType supertype)
	{
		return (type, supertype) switch
		{
			(TokenType.Integer, TokenType.Float) => true,
			_ => type == supertype
		};
	}
}