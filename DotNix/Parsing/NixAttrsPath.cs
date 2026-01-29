namespace DotNix.Parsing;

public record NixAttrsPath(PosSpan Span, NixIdentifier Identifier);