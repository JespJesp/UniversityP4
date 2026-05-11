global using Xunit;
global using Shouldly;

// Run all tests: dotnet test
// Run unit tests: dotnet test --filter "Category!=Integration"
// Run integration tests: dotnet test --filter "Category=Integration"
// Run specific test: dotnet test --filter "FullyQualifiedName~ParserTests"
// Run with verbose output: dotnet test -v normal

// Can also run the test by hovering it or going to the test tab (I think it's from a test explorer extension)