using FunicularSwitch.Generators;

namespace DotNix.Parsing;

[ResultType(typeof(ParserError))]
public abstract partial class ParserResult<T>;

public record ParserError(string Message);