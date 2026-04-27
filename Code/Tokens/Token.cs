namespace Tokens;

public class Token
{
	public TokenType Type { get; }
	public string Value { get; }
	public CursorInfo CursorInfo = new();

	public Token(TokenType type, string value = "")
	{
		Type = type;
		Value = value;
	}

	public Token(TokenType type, string value, string fileName, int line, int column)
	{
		Type = type;
		Value = value;
		CursorInfo = new(fileName, line, column);
	}
}