using AwesomeAssertions;
using AwesomeAssertions.Execution;
using DotNix.Parsing;
using DotNix.Parsing.Models;
using DotNix.Utils.TreeSitter;
using LanguageExt.Parsec;
using static DotNix.Utils.TreeSitter.TreeSitterNode;

namespace DotNix.Tests.Utils.TreeSitter;

[TestClass]
public class TreeSitterParserTest
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public TestContext TestContext { get; set; } = null!;

    private CancellationToken CancellationToken => TestContext.CancellationTokenSource.Token;
    
    [DataTestMethod]
    [DynamicData(nameof(TestCases))]
    public async Task ParseCode(string code, TreeSitterNode expectedExpression)
    {
        // Arrange
        await using var grammarJson = typeof(NixParser).Assembly.GetManifestResourceStream(typeof(NixParser), "grammar.json") ?? throw new InvalidOperationException();
        await using var nodeTypesJson = typeof(NixParser).Assembly.GetManifestResourceStream(typeof(NixParser), "node-types.json") ?? throw new InvalidOperationException();

        // Act
        var parser = await TreeSitterParser.Create(grammarJson, nodeTypesJson, CancellationToken);
        var result = parser.Parse(code, CancellationToken);

        // Assert
        if(result.IsFaulted)
            Assert.Fail(result.Reply.Error!.ToString());
        result.Should().Be(expectedExpression);
    }

    public static IEnumerable<object[]> TestCases => TestCasesTyped.Select(x => new object[] { x.Item1, x.Item2 });

    private static IEnumerable<(string, TreeSitterNode)> TestCasesTyped =>
    [
        (
            /*lang=nix*/"""
                        true
                        """,
            Blank
        ),
        // (
        //     /*lang=nix*/"""
        //                 1337
        //                 """,
        //     Blank
        // ),
        // (
        //     /*lang=nix*/"""
        //                 1337.42
        //                 """,
        //     Blank
        // ),
        // (
        //     /*lang=nix*/"""
        //                 "hi"
        //                 """,
        //     Blank
        // ),
        // (
        //     /*lang=nix*/"""
        //                 "answer: ${"42"}"
        //                 """,
        //     Blank
        // ),
        // (
        //     /*lang=nix*/"""
        //                 "answer: \${\"42\"}"
        //                 """,
        //     Blank
        // ),
        // (
        //     /*lang=nix*/"""
        //                 ''
        //                 hi
        //                 ''
        //                 """,
        //     Blank
        // ),
        // (
        //     /*lang=nix*/"""
        //                 ''
        //                 answer: ${''
        //                 42
        //                 ''}
        //                 ''
        //                 """,
        //     Blank
        // ),
        // (
        //     /*lang=nix*/"""
        //                 ''
        //                 answer: ''${'''
        //                 42
        //                 '''}
        //                 ''
        //                 """,
        //     Blank
        // ),
    ];
}