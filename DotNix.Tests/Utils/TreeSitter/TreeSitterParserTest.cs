using AwesomeAssertions;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Extensibility;
using DotNix.Parsing;
using DotNix.Parsing.Models;
using DotNix.Tests.Utils.TreeSitter;
using DotNix.Utils.TreeSitter;
using LanguageExt;
using LanguageExt.Parsec;
using static DotNix.Utils.TreeSitter.TreeSitterNode;

[assembly: AssertionEngineInitializer(typeof(Initializer), nameof(Initializer.Initialize))]

namespace DotNix.Tests.Utils.TreeSitter;

public static class Initializer
{
    public static void Initialize()
    {
        AssertionConfiguration.Current.Equivalency.Modify(options => options
            .Using<Map<string, Lst<TreeSitterNode>>>(x => x.Subject.Pairs.ToDictionary().Should().BeEquivalentTo(x.Expectation.Pairs.ToDictionary())).WhenTypeIs<Map<string, Lst<TreeSitterNode>>>()
        );
    }
}


[TestClass, Ignore]
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
        result.Reply.Result.Should()
            .BeEquivalentTo(expectedExpression, options => options
                .Excluding(x => x.Types));
    }

    public static IEnumerable<object[]> TestCases => TestCasesTyped.Select(x => new object[] { x.Item1, x.Item2 });

    private static IEnumerable<(string, TreeSitterNode)> TestCasesTyped =>
    [
        (
            /*lang=nix*/"""
                        true
                        """,
            Token("variable_expression", "true")
                .AddField("name", Token("identifier", "true"))
        ),
        (
            /*lang=nix*/"""
                        1337
                        """,
            Token("integer_expression", "1337")
        ),
        (
            /*lang=nix*/"""
                        1337.42
                        """,
            Token("float_expression", "1337.42")
        ),
        (
            /*lang=nix*/"""
                        "hi"
                        """,
            Token("string_expression", "hi")
        ),
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