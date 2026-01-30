namespace DotNix.Compiling;

public static class Builtins
{
    public static NixAttrs AsAttrs => field ??= new NixAttrs(Map<string, NixValue2>(
        ("add", AddFn)
    ));
    
    private static NixFunction AddFn => OfType<NixNumber, NixNumber>(Add);

    private static NixFunction OfType<A, B>(Func<A, B, NixValue2> fn)
        where A : NixValue2
        where B : NixValue2
        => OfType((A a) => OfType((B b) => fn(a, b)));
    
    private static NixFunction OfType<A>(Func<A, NixValue2> fn)
        where A : NixValue2
        => new(async arg =>
            (await arg.Strict) is not A argTyped
                ? throw new NotSupportedException()
                : fn(argTyped)
        );
    
    public static NixNumber Add(NixNumber a, NixNumber b) =>
        CalcOp(
            (a1, b1) => new(a1.Value + b1.Value),
            (a1, b1) => new(a1.Value + b1.Value),
            a, b);

    public static NixNumber Sub(NixNumber a, NixNumber b) =>
        CalcOp(
            (a1, b1) => new(a1.Value - b1.Value),
            (a1, b1) => new(a1.Value - b1.Value),
            a, b);
    
    public static NixNumber Mul(NixNumber a, NixNumber b) =>
        CalcOp(
            (a1, b1) => new(a1.Value * b1.Value),
            (a1, b1) => new(a1.Value * b1.Value),
            a, b);

    public static NixNumber Div(NixNumber a, NixNumber b) =>
        CalcOp(
            (a1, b1) => new(a1.Value / b1.Value),
            (a1, b1) => new(a1.Value / b1.Value),
            a, b);

    private static NixNumber CalcOp(Func<NixInteger, NixInteger, NixInteger> integerFn, Func<NixFloat, NixFloat, NixFloat> floatFn, NixNumber a, NixNumber b) =>
        a is NixInteger aInt
            ? b is NixInteger bInt
                ? integerFn(aInt, bInt)
                : floatFn(aInt.AsFloat, (NixFloat) b)
            : b is NixInteger bInt2
                ? floatFn((NixFloat) a, bInt2.AsFloat)
                : floatFn((NixFloat) a, (NixFloat) b);
}