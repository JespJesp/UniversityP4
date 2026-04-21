using System.Reflection;
using Ast;
using Lexing.Tokens;

namespace UniversityP4.Tests;

public class ParserTests
{
    [Fact]
    public void TryConsumeToken_Should_Accept_Integer_When_Expecting_Float()
    {
        InitializeParserState(
            new Token(TokenType.Integer, "7"),
            new Token(TokenType.EndOfFile));

        string? consumedValue = null;

        var consumed = Parser.TryConsumeToken(TokenType.Float, value => consumedValue = value);

        consumed.ShouldBeTrue();
        consumedValue.ShouldBe("7");
        Parser.CurrentToken.Type.ShouldBe(TokenType.EndOfFile);
    }

    [Fact]
    public void ConsumeToken_Should_Throw_When_CurrentToken_Does_Not_Match()
    {
        InitializeParserState(
            new Token(TokenType.Identifier, "_abc"),
            new Token(TokenType.EndOfFile));

        Action act = () => Parser.ConsumeToken(TokenType.String);

        var exception = Should.Throw<Exception>(act);

        exception.Message.ShouldContain("Expected token of type 'String'");
    }

    [Fact]
    public void TryConsumeIndent_Should_Return_True_And_Advance_When_Newline_And_Indent_Match()
    {
        InitializeParserState(
            new Token(TokenType.Newline),
            new Token(TokenType.Indent, "2"),
            new Token(TokenType.EndOfFile));

        var consumed = Parser.TryConsumeIndent(2);

        consumed.ShouldBeTrue();
        Parser.CurrentToken.Type.ShouldBe(TokenType.EndOfFile);
    }

    [Fact]
    public void TryConsumeTokens_Should_Return_False_And_Not_Advance_When_Value_Does_Not_Match()
    {
        InitializeParserState(
            new Token(TokenType.Integer, "8"),
            new Token(TokenType.EndOfFile));

        var consumed = Parser.TryConsumeTokens(new[] { new Token(TokenType.Float, "7") });

        consumed.ShouldBeFalse();
        Parser.CurrentToken.Type.ShouldBe(TokenType.Integer);
        Parser.CurrentToken.Value.ShouldBe("8");
    }

    [Fact]
    public void HandleUniqueOptions_Should_Consume_Options_In_Any_Order_When_Separated()
    {
        InitializeParserState(
            new Token(TokenType.Identifier, "_lead"),
            new Token(TokenType.Comma),
            new Token(TokenType.Integer, "42"),
            new Token(TokenType.EndOfFile));

        string? parsedId = null;
        string? parsedNumber = null;

        Parser.HandleUniqueOptions(
            new Dictionary<TokenType, Action>
            {
                [TokenType.Identifier] = () => Parser.ConsumeToken(TokenType.Identifier, value => parsedId = value),
                [TokenType.Integer] = () => Parser.ConsumeToken(TokenType.Integer, value => parsedNumber = value)
            },
            new[] { new Token(TokenType.Comma) });

        parsedId.ShouldBe("_lead");
        parsedNumber.ShouldBe("42");
        Parser.CurrentToken.Type.ShouldBe(TokenType.EndOfFile);
    }

    [Fact]
    public void HandleUniqueOptions_Should_Throw_When_Option_Is_Duplicated()
    {
        InitializeParserState(
            new Token(TokenType.Integer, "1"),
            new Token(TokenType.Comma),
            new Token(TokenType.Integer, "2"),
            new Token(TokenType.EndOfFile));

        Action act = () => Parser.HandleUniqueOptions(
            new Dictionary<TokenType, Action>
            {
                [TokenType.Integer] = () => Parser.ConsumeToken(TokenType.Integer)
            },
            new[] { new Token(TokenType.Comma) });

        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldContain("Duplicate optional token 'Integer'");
    }

    private static void InitializeParserState(params Token[] tokens)
    {
        typeof(Parser)
            .GetField("_tokens", BindingFlags.NonPublic | BindingFlags.Static)!
            .SetValue(null, tokens.ToList());

        typeof(Parser)
            .GetField("_cursorPosition", BindingFlags.NonPublic | BindingFlags.Static)!
            .SetValue(null, 0);

        typeof(Parser)
            .GetField("_syntaxErrors", BindingFlags.NonPublic | BindingFlags.Static)!
            .SetValue(null, new List<string>());
    }
}
