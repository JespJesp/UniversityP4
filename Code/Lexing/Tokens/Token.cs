namespace Lexing.Tokens;

public class Token
{
	public TokenType Type { get; }
	public string Value { get; }
	public int Line { get; }
	public int Column { get; }

	public Token(TokenType type, string value = "", int line = -1, int column = -1)
	{
		Type = type;
		Value = value;
		Line = line;
		Column = column;
	}

	public override string ToString()
	{
		return $"Token(Type: {Type}, Value: '{Value}', Line: {Line}, Column: {Column})";
	}
}