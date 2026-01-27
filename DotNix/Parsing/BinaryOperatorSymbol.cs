using System;

namespace DotNix.Parsing;

public record BinaryOperatorSymbol(PosSpan Span, BinaryOperator Operator);
