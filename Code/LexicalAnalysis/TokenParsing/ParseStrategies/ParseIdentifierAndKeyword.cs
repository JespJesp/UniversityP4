

namespace LexicalAnalysis.TokenParsing.ParseStrategies;

public class ParseIdentifierAndKeyword : ITokenParseStrategy
{
	public bool IsParsable(TokenizationOperation op)
	{
		return char.IsLetter(op.CurrentChar) || op.CurrentChar == '_';
	}

	public void Parse(TokenizationOperation op)
	{
		string identifier = "";
		int startCol = op.CurrentColumn;
		while (op.CurrentPosition < op.Input.Length && (char.IsLetterOrDigit(op.Input[op.CurrentPosition]) || op.Input[op.CurrentPosition] == '_'))
		{
			identifier += op.Input[op.CurrentPosition];
			op.CurrentPosition++;
			op.CurrentColumn++;
		}

		TokenType type = identifier switch
		{
			"samples" => TokenType.SampleKeyword,
			"notes" => TokenType.NotesKeyword,
			_ => TokenType.Identifier
		};

		op.Tokens.Add(new Token(type, identifier, op.CurrentLine, startCol));
	}
}