
using AwesomeAssertions;
using CliWrap;
using CliWrap.Buffered;
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

    private static object? ToJson(NixValue value)
    {
        return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(ToIntermediateValue(value)));
        
        static object ToIntermediateValue(NixValue value) => value.Match<object>(
            integer => integer.Value
        );
    }
}
