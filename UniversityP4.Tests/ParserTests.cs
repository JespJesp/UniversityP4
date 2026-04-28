using System.Reflection;
using Phases.Parsing;
using Tokens;

namespace UniversityP4.Tests;

public class ParserTests
{
    [Fact]
    public void TryConsumeToken_Should_Accept_Integer_When_Expecting_Float()
    {
        var parser = CreateParser(
            new Token(TokenType.Integer, "7"),
            new Token(TokenType.EndOfFile));

        var consumed = parser.TryConsumeToken(TokenType.Float, out string consumedValue);

        consumed.ShouldBeTrue();
        consumedValue.ShouldBe("7");
        parser.CursorToken.Type.ShouldBe(TokenType.EndOfFile);
    }

    [Fact]
    public void ConsumeToken_Should_Throw_When_CurrentToken_Does_Not_Match()
    {
        var parser = CreateParser(
            new Token(TokenType.Identifier, "_abc"),
            new Token(TokenType.EndOfFile));

        Action act = () => parser.ConsumeToken(TokenType.String);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("Expected token of type 'String'");
    }

    [Fact]
    public void TryConsumeIndent_Should_Return_True_And_Advance_When_Newline_And_Indent_Match()
    {
        var parser = CreateParser(
            new Token(TokenType.Newline),
            new Token(TokenType.Indent, "2"),
            new Token(TokenType.EndOfFile));

        var consumed = parser.TryConsumeIndent(2);

        consumed.ShouldBeTrue();
        parser.CursorToken.Type.ShouldBe(TokenType.EndOfFile);
    }

    [Fact]
    public void TryConsumeTokens_Should_Return_False_And_Not_Advance_When_Value_Does_Not_Match()
    {
        var parser = CreateParser(
            new Token(TokenType.Integer, "8"),
            new Token(TokenType.EndOfFile));

        var consumed = parser.TryConsumeTokens(new[] { new Token(TokenType.Float, "7") });

        consumed.ShouldBeFalse();
        parser.CursorToken.Type.ShouldBe(TokenType.Integer);
        parser.CursorToken.Value.ShouldBe("8");
    }

    [Fact]
    public void TryConsumeOptions_Should_Consume_Options_In_Any_Order_When_Separated()
    {
        var parser = CreateParser(
            new Token(TokenType.Identifier, "_lead"),
            new Token(TokenType.Comma),
            new Token(TokenType.Integer, "42"),
            new Token(TokenType.EndOfFile));

        string? parsedId = null;
        string? parsedNumber = null;

        parser.TryConsumeOptions(
            new()
            {
                (
                    () =>
                    {
                        if (parser.TryConsumeToken(TokenType.Identifier, out string value))
                        {
                            parsedId = value;
                            return true;
                        }

                        return false;
                    },
                    () => { }
                ),
                (
                    () =>
                    {
                        if (parser.TryConsumeToken(TokenType.Integer, out string value))
                        {
                            parsedNumber = value;
                            return true;
                        }

                        return false;
                    },
                    () => { }
                ),
            },
            [new Token(TokenType.Comma)]);

        parsedId.ShouldBe("_lead");
        parsedNumber.ShouldBe("42");
        parser.CursorToken.Type.ShouldBe(TokenType.EndOfFile);
    }

    [Fact]
    public void TryConsumeOptions_Should_Stop_When_Encountering_Duplicate_Option()
    {
        var parser = CreateParser(
            new Token(TokenType.Integer, "1"),
            new Token(TokenType.Comma),
            new Token(TokenType.Integer, "2"),
            new Token(TokenType.EndOfFile));

        string? parsedNumber = null;

        parser.TryConsumeOptions(
            new()
            {
                (
                    () =>
                    {
                        if (parser.TryConsumeToken(TokenType.Integer, out string value))
                        {
                            parsedNumber = value;
                            return true;
                        }

                        return false;
                    },
                    () => { }
                ),
            },
            [new Token(TokenType.Comma)]);

        parsedNumber.ShouldBe("1");
        parser.CursorToken.Type.ShouldBe(TokenType.Integer);
        parser.CursorToken.Value.ShouldBe("2");
    }

    private static Parser CreateParser(params Token[] tokens)
    {
        var parser = new Parser();

        typeof(Parser)
            .GetField("_tokens", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(parser, tokens.ToList());

        typeof(Parser)
            .GetField("_cursorPosition", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(parser, 0);

        typeof(Parser)
            .GetField("_errors", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(parser, new List<string>());

        return parser;
    }
}
