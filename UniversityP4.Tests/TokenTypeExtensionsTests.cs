using Tokens;

namespace UniversityP4.Tests;

public class TokenTypeExtensionsTests
{
    [Fact]
    public void IsSubtypeOf_Should_Treat_Integer_As_Float_Subtype()
    {
        TokenType.Integer.IsSubtypeOf(TokenType.Float).ShouldBeTrue();
    }

    [Fact]
    public void IsSubtypeOf_Should_Not_Treat_Float_As_Integer_Subtype()
    {
        TokenType.Float.IsSubtypeOf(TokenType.Integer).ShouldBeFalse();
    }

    [Fact]
    public void IsSubtypeOf_Should_Return_True_For_Exact_Type_Match()
    {
        TokenType.Identifier.IsSubtypeOf(TokenType.Identifier).ShouldBeTrue();
    }
}
