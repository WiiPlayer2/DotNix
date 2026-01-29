namespace DotNix.Parsing;

public record PosSpan(Pos Begin, Pos End)
{
    public static PosSpan operator |(PosSpan left, PosSpan right) =>
        new(left.Begin, right.End);

    public override string ToString() => $"({Begin.Line},{Begin.Column}-{End.Line},{End.Column})";
}
