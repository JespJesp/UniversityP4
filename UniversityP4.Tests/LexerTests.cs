using Lexing;
using Lexing.Tokens;

namespace UniversityP4.Tests;

public class LexerTests
{
    [Fact]
    public void Lex_Should_Tokenize_Integers_And_Floats()
    {
        var tokens = Lexer.Lex("42 -7 3.14");

        tokens.Count.ShouldBe(4);
        tokens.Select(t => t.Type).ShouldBe(new[]
        {
            TokenType.Integer,
            TokenType.Integer,
            TokenType.Float,
            TokenType.EndOfFile
        });
        tokens[0].Value.ShouldBe("42");
        tokens[1].Value.ShouldBe("-7");
        tokens[2].Value.ShouldBe("3.14");
    }

    [Fact]
    public void Lex_Should_Emit_Newline_And_Indent_Tokens_For_Tabs()
    {
        var tokens = Lexer.Lex("\n\t\t42");

        tokens.Select(t => t.Type).ShouldBe(new[]
        {
            TokenType.Newline,
            TokenType.Indent,
            TokenType.Integer,
            TokenType.EndOfFile
        });
        tokens[1].Value.ShouldBe("2");
    }

    [Fact]
    public void Lex_Should_Throw_When_Number_Contains_Multiple_Decimal_Points()
    {
        Action act = () => _ = Lexer.Lex("3.14.159");

        var exception = Should.Throw<Exception>(act);

        exception.Message.ShouldContain("multiple decimal symbols");
    }

    [Fact]
    public void Lex_Should_Reset_Previous_Errors_Between_Calls()
    {
        Action failingLex = () => _ = Lexer.Lex("3.14.159");
        Should.Throw<Exception>(failingLex);

        var tokens = Lexer.Lex("42");

        tokens.Select(t => t.Type).ShouldBe(new[] { TokenType.Integer, TokenType.EndOfFile });
        tokens[0].Value.ShouldBe("42");
    }
}
