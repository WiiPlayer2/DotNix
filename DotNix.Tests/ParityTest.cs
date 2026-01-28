
using AwesomeAssertions;
using CliWrap;
using CliWrap.Buffered;
using DotNix.Compiling;
using Newtonsoft.Json;

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
        var expected = JsonConvert.DeserializeObject(cliResult.StandardOutput);

        // Act
        var result = ToJson(await Nix.EvalExpr(code));

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    [DataRow("01-calc.nix")]
    public async Task EvalFile(string file)
    {
        // Arrange
        var filePath = Path.GetFullPath($"./tests/{file}");
        var cliResult = await Cli.Wrap("nix")
            .WithArguments(["eval", "--json", "--file", filePath])
            .ExecuteBufferedAsync();
        var expected = JsonConvert.DeserializeObject(cliResult.StandardOutput);

        // Act
        var fileContent = await File.ReadAllTextAsync(filePath);
        var result = ToJson(await Nix.EvalExpr(fileContent));

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    private static object? ToJson(NixValue value)
    {
        return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ToIntermediateValue(value)));
        
        static object ToIntermediateValue(NixValue value) => value.Match<object>(
            integer => integer.Value
        );
    }

    private static object? ToJson(NixValue2 value)
    {
        return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ToIntermediateValue(value)));

        static object ToIntermediateValue(NixValue2 value) => value switch
        {
            NixInteger integer => integer.Value,
        };
    }
}
