using System.Collections;
using DotNix.Types;

namespace DotNix.Compiling;

public static class Operators
{
    public static NixValue Plus(NixValue a, NixValue b) => Builtins.Add((NixNumber)a, (NixNumber)b);

    public static NixValue Minus(NixValue a, NixValue b) => Builtins.Sub((NixNumber)a, (NixNumber)b);
    
    public static NixValue Mul(NixValue a, NixValue b) => Builtins.Mul((NixNumber)a, (NixNumber)b);
    
    public static NixValue Div(NixValue a, NixValue b) => Builtins.Div((NixNumber)a, (NixNumber)b);

    public static NixValue Negate(NixValue a) =>
        a switch
        {
            NixInteger aInt => new NixInteger(-aInt.Value),
            NixFloat aFloat => new NixFloat(-aFloat.Value),
            _ => throw new NotSupportedException(),
        };

    public static NixValue And(NixValue a, NixValue b) => BoolOp(a, b, (a, b) => a && b);
    
    public static NixValue Or(NixValue a, NixValue b) => BoolOp(a, b, (a, b) => a || b);
    
    public static NixValue Impl(NixValue a, NixValue b) => BoolOp(a, b, (a, b) => !a || b);
    
    private static NixBool BoolOp(NixValue a, NixValue b, Func<bool, bool, bool> fn) =>
        a switch
        {
            NixBool aBool => b switch
            {
                NixBool bBool => (NixBool) fn(aBool.Value, bBool.Value),
            },
        };

    public static NixValue Not(NixValue a) =>
        a switch
        {
            NixBool aInt => (NixBool) !(aInt.Value),
        };

    public static NixValue Equal(NixValue a, NixValue b) =>
        (NixBool)(Compare(a, b) == 0);

    public static NixValue NotEqual(NixValue a, NixValue b) =>
        (NixBool)(Compare(a, b) != 0);

    public static NixValue LessThan(NixValue a, NixValue b) =>
        (NixBool)(Compare(a, b) < 0);

    public static NixValue LessThanOrEqual(NixValue a, NixValue b) =>
        (NixBool)(Compare(a, b) <= 0);

    public static NixValue GreaterThan(NixValue a, NixValue b) =>
        (NixBool)(Compare(a, b) > 0);

    public static NixValue GreaterThanOrEqual(NixValue a, NixValue b) =>
        (NixBool)(Compare(a, b) >= 0);

    private static int? Compare(NixValue a, NixValue b) =>
        a switch
        {
            NixInteger aInt => b switch
            {
                NixInteger bInt => Comparer.Default.Compare(aInt.Value, bInt.Value),
                NixFloat bFloat => Comparer.Default.Compare((double) aInt.Value, bFloat.Value),
                _ => null,
            },
            NixFloat aFloat => b switch
            {
                NixInteger bInt => Comparer.Default.Compare(aFloat.Value, (double) bInt.Value),
                NixFloat bFloat => Comparer.Default.Compare(aFloat.Value, bFloat.Value),
                _ => null,
            },
            _ => null,
        };

    public static NixValue Concat(NixValue a, NixValue b)
    {
        var aList = (NixList) a;
        var bList = (NixList) b;
        var items = aList.Items.Concat(bList.Items).ToList();
        return new NixList(items);
    }

    public static NixValue Update(NixValue a, NixValue b)
    {
        var aAttrs = (NixAttrs) a;
        var bAttrs = (NixAttrs) b;
        var items = aAttrs.Items.ToDictionary();
        foreach (var item in bAttrs.Items.ToDictionary())
            items[item.Key] = item.Value;
        return new NixAttrs(items);
    }
}