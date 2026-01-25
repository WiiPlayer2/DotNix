using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;

namespace DotNix;

public static class Nix
{
    public static async Task<NixValue> EvalExpr(string code)
    {
        var parser = CreateParser();
        var result = parse(parser, code);
        if (result.IsFaulted)
            throw new Exception(result.Reply.Error?.ToString());

        var value = await EvaluateToValue(result.Reply.Result!);
        return value;
    }

    private static Parser<NixExpr> CreateParser()
    {
        var nixLanguageDef = GenLanguageDef.Empty.With(
            ReservedOpNames: ["+", "-"]
        );
        
        var nixTokenParser = Token.makeTokenParser(nixLanguageDef);

        var addOp = Operator.Infix<NixExpr>(
            Assoc.Left,
            from _10 in nixTokenParser.ReservedOp("+")
            select new Func<NixExpr, NixExpr, NixExpr>(NixExpr.AddOp)
        );
        var subOp = Operator.Infix<NixExpr>(
            Assoc.Left,
            from _10 in nixTokenParser.ReservedOp("-")
            select new Func<NixExpr, NixExpr, NixExpr>(NixExpr.SubOp)
        );

        Operator<NixExpr>[][] table = [
            [ addOp, subOp ],
        ];

        var integerExpr = nixTokenParser.Integer.Map(x => NixExpr.Literal(NixValue.Integer(x)));
        var literal = integerExpr;
        
        Parser<NixExpr> expr = null!;
        // ReSharper disable once AccessToModifiedClosure
        var term = either(nixTokenParser.Parens(lazyp(() => expr)), literal);
        expr = Expr.buildExpressionParser(table, term);

        return expr;
    }

    private static Task<NixExpr> Evaluate(NixExpr expr) => expr.Match<Task<NixExpr>>(
        addOp: op =>
            from left in EvaluateToValue(op.Left)
            from right in EvaluateToValue(op.Right)
            select left.Match(
                integer: a => right.Match(
                    integer: b =>  NixExpr.Literal(NixValue.Integer(a.Value + b.Value))
                )
            ),
        subOp: op =>
            from left in EvaluateToValue(op.Left)
            from right in EvaluateToValue(op.Right)
            select left.Match(
                integer: a => right.Match(
                    integer: b =>  NixExpr.Literal(NixValue.Integer(a.Value - b.Value))
                )
            ),
        literal: _ => Task.FromResult(expr)
    );

    private static async Task<NixValue> EvaluateToValue(NixExpr expr)
    {
        var currentExpr = expr;
        while(currentExpr is not NixExpr.Literal_)
            currentExpr = await Evaluate(currentExpr);
        return ((NixExpr.Literal_) currentExpr).Value;
    }
}