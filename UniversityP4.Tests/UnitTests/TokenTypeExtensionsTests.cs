using Tokens;

namespace UniversityP4.Tests;

[Trait("Category","Unit")]
public class TokenTypeExtensionsTests
{
    [Fact]
    public void TokenType_Equals_Returns_True_For_Same_Type()
    {
        TokenType.Identifier.ShouldBe(TokenType.Identifier);
    }
}
