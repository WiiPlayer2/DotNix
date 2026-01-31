namespace DotNix.Compiling;

public static class Builtins
{
    public static NixAttrs AsAttrs => field ??= new NixAttrs(Map<string, NixValue2>(
        ("add", AddFn),
        ("true", True),
        ("false", False),
        ("concatMap", ConcatMapFn),
        ("map", MapFn)
    ));
    
    private static NixFunction AddFn => OfType<NixNumber, NixNumber>(Add);
    
    private static NixFunction ConcatMapFn => OfType<NixFunction, NixList, NixList>(ConcatMap);

    private static NixFunction MapFn => OfType<NixFunction, NixList, NixList>(Map);

    public static NixBool True => NixBool.True;
    
    public static NixBool False => NixBool.False;

    private static NixFunction OfType<A, B>(Func<A, B, NixValue2> fn)
        where A : NixValue2
        where B : NixValue2
        => OfType((A a) => OfType((B b) => fn(a, b)));

    private static NixFunction OfType<A, B, C>(Func<A, B, Task<C>> fn)
        where A : NixValue2
        where B : NixValue2
        where C : NixValue2
        => OfType((A a) => OfType((B b) => fn(a, b)));

    private static NixFunction OfType<A>(Func<A, NixValue2> fn)
        where A : NixValue2
        => new(async arg =>
            (await arg.UnThunk) is not A argTyped
                ? throw new NotSupportedException()
                : fn(argTyped)
        );
    
    private static NixFunction OfType<A, B>(Func<A, Task<B>> fn)
        where A : NixValue2
        where B : NixValue2
        => new(async arg =>
            (await arg.UnThunk) is not A argTyped
                ? throw new NotSupportedException()
                : await fn(argTyped)
        );

    public static async Task<NixList> ConcatMap(NixFunction fn, NixList list) => new(
        (await Task.WhenAll(list.Items.Select(async x => ((NixList)await (await fn.Fn(x)).UnThunk)))).SelectMany(x => x.Items).ToList()
    );

    public static async Task<NixList> Map(NixFunction fn, NixList list) => new(
        await Task.WhenAll(list.Items.Select(async x => await fn.Fn(x)))
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