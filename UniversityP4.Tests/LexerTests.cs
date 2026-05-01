using Phases.Lexing;
using Tokens;

namespace UniversityP4.Tests;

public class LexerTests
{
    private static readonly FileInfo BaseFile = new(Path.Combine(Path.GetTempPath(), "UniversityP4.Tests.mude"));

    [Fact]
    public void Lex_Should_Tokenize_Integers_And_Floats()
    {
		var tokens = new Lexer().Lex("42 -7 3.14", BaseFile);

                tokens.Count.ShouldBe(3);
        tokens.Select(t => t.Type).ShouldBe(new[]
        {
            TokenType.Integer,
            TokenType.Integer,
            TokenType.Float
        });
        tokens[0].Value.ShouldBe("42");
        tokens[1].Value.ShouldBe("-7");
        tokens[2].Value.ShouldBe("3.14");
    }

    [Fact]
    public void Lex_Should_Emit_Newline_And_Indent_Tokens_For_Tabs()
    {
		var tokens = new Lexer().Lex("\n\t\t42", BaseFile);

        tokens.Select(t => t.Type).ShouldBe(new[]
        {
            TokenType.Newline,
            TokenType.Indent,
            TokenType.Integer
        });
        tokens[1].Value.ShouldBe("2");
    }

    [Fact]
    public void Lex_Should_Throw_When_Number_Contains_Multiple_Decimal_Points()
    {
		Action act = () => _ = new Lexer().Lex("3.14.159", BaseFile);

        var exception = Should.Throw<Exception>(act);

        exception.Message.ShouldContain("multiple decimal symbols");
    }

    [Fact]
    public void Lex_Should_Reset_Previous_Errors_Between_Calls()
    {
        Action failingLex = () => _ = new Lexer().Lex("3.14.159", BaseFile);
        Should.Throw<Exception>(failingLex);

        var tokens = new Lexer().Lex("42", BaseFile);

        tokens.Select(t => t.Type).ShouldBe(new[] { TokenType.Integer });
        tokens[0].Value.ShouldBe("42");
    }
}
