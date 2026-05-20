using System.Reflection;
using Phases.Parsing;
using Tokens;

namespace UniversityP4.Tests;

[Trait("Category","Unit")]
public class ParserTests
{
    [Fact]
    public void TryConsumeToken_Should_Not_Accept_Different_Types()
    {
        var parser = CreateParser(
            new Token(TokenType.Float, "7"));

        var consumed = parser.TryConsumeToken(TokenType.Identifier, out string consumedValue);

        consumed.ShouldBeFalse();
        consumedValue.ShouldBeEmpty();
        parser.AtEndOfTokens.ShouldBeFalse();
    }

    [Fact]
    public void ConsumeToken_Should_Throw_When_CurrentToken_Does_Not_Match()
    {
        var parser = CreateParser(
            new Token(TokenType.Identifier, "_abc"));

        Action act = () => parser.ConsumeToken(TokenType.String);

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("Expected token of type 'String'");
    }

    [Fact]
    public void TryConsumeNewlineIndent_Should_Return_True_And_Advance_When_Newline_And_Indent_Match()
    {
        var parser = CreateParser(
            new Token(TokenType.Newline),
            new Token(TokenType.Indent, "2"));

        var consumed = parser.TryConsumeNewlineIndent(2);

        consumed.ShouldBeTrue();
        parser.AtEndOfTokens.ShouldBeTrue();
    }

    [Fact]
    public void TryConsumeTokens_Should_Return_False_And_Not_Advance_When_Value_Does_Not_Match()
    {
        var parser = CreateParser(
            new Token(TokenType.Float, "8"));

        var consumed = parser.TryConsumeTokens(new[] { new Token(TokenType.Float, "7") });

        consumed.ShouldBeFalse();
        parser.CursorToken.Type.ShouldBe(TokenType.Float);
        parser.CursorToken.Value.ShouldBe("8");
    }

    [Fact]
    public void TryConsumeOptions_Should_Consume_Options_In_Any_Order_When_Separated()
    {
        var parser = CreateParser(
            new Token(TokenType.Identifier, "_lead"),
            new Token(TokenType.Comma),
            new Token(TokenType.Float, "42"));

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
                        if (parser.TryConsumeToken(TokenType.Float, out string value))
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
        parser.AtEndOfTokens.ShouldBeTrue();
    }

    [Fact]
    public void TryConsumeOptions_Should_Stop_When_Encountering_Duplicate_Option()
    {
        var parser = CreateParser(
            new Token(TokenType.Float, "1"),
            new Token(TokenType.Comma),
            new Token(TokenType.Float, "2"));

        string? parsedNumber = null;

        parser.TryConsumeOptions(
            new()
            {
                (
                    () =>
                    {
                        if (parser.TryConsumeToken(TokenType.Float, out string value))
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
        parser.CursorToken.Type.ShouldBe(TokenType.Float);
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
