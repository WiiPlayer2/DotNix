
using System.Linq.Expressions;
using AwesomeAssertions.Json;
using CliWrap;
using CliWrap.Buffered;
using DotNix.Compiling;
using DotNix.Parsing;
using LanguageExt;
using LanguageExt.Parsec;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Char = LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;

namespace DotNix.Tests;

[TestClass]
public class ParityTest
{
    [TestMethod]
    [DataRow("3")]
    [DataRow("7")]
    [DataRow("1 + 2")]
    [DataRow("4 + 13")]
    [DataRow("4 - 2")]
    [DataRow("(10 + 13) - 22")]
    public async Task EvalExpr(string code)
    {
        // Arrange
        var cliResult = await Cli.Wrap("nix")
            .WithArguments(["eval", "--json", "--expr", code])
            .ExecuteBufferedAsync();
        var expected = JsonConvert.DeserializeObject<JToken?>(cliResult.StandardOutput);

        // Act
        var result = ToJson(await Nix.EvalExpr(code));

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [DataTestMethod]
    [DynamicData(nameof(TestFiles))]
    public async Task EvalFile(string file)
    {
        // Arrange
        var filePath = Path.GetFullPath($"./tests/{file}");
        var cliResult = await Cli.Wrap("nix")
            .WithArguments(["eval", "--json", "--file", filePath])
            .ExecuteBufferedAsync();
        var expected = JsonConvert.DeserializeObject<JToken?>(cliResult.StandardOutput);

        // Act
        var fileContent = await File.ReadAllTextAsync(filePath);
        var result = ToJson(await Nix.EvalExpr(fileContent));

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    public static IEnumerable<object[]> TestFiles =>
        Directory.EnumerateFiles("./tests")
            .Select(x => new object[] { new FileInfo(x).Name });

    private static JToken? ToJson(NixValue2 value)
    {
        return JsonConvert.DeserializeObject<JToken?>(JsonConvert.SerializeObject(ToIntermediateValue(value)));

        static object ToIntermediateValue(NixValue2 value) => value switch
        {
            NixInteger integer => integer.Value,
            NixList list => list.Items.Select(ToIntermediateValue),
            NixFloat @float => @float.Value,
            NixAttrs attrs => attrs.Items
                .Select(kv => new KeyValuePair<string,object>(kv.Key, ToIntermediateValue(kv.Value)))
                .ToDictionary(),
        };
    }
}
