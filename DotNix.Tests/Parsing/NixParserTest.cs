using AwesomeAssertions;
using AwesomeAssertions.Execution;
using DotNix.Parsing;
using DotNix.Parsing.Models;
using static DotNix.Parsing.Models.NixExpression;

namespace DotNix.Tests.Parsing;

[TestClass]
public class NixParserTest
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public TestContext TestContext { get; set; } = null!;

    private CancellationToken CancellationToken => TestContext.CancellationTokenSource.Token;
    
    [DataTestMethod]
    [DynamicData(nameof(TestCases))]
    public void ParseCode(string code, ParserResult<NixExpression> expectedExpression)
    {
        // Arrange

        // Act
        var result = NixParser.Parse(code, CancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.GetValueOrDefault().Should().Be(expectedExpression.GetValueOrDefault());
            result.GetErrorOrDefault().Should().Be(expectedExpression.GetErrorOrDefault());
        }
    }

    public static IEnumerable<object[]> TestCases => TestCasesTyped.Select(x => new object[] { x.Item1, x.Item2 });

    private static IEnumerable<(string, ParserResult<NixExpression>)> TestCasesTyped =>
    [
        (
            /*lang=nix*/"""
                        true
                        """,
            Variable("true")
        ),
        (
            /*lang=nix*/"""
                        1337
                        """,
            Integer(1337)
        ),
    ];
}