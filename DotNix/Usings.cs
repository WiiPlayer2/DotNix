global using LanguageExt;
global using LanguageExt.Parsec;
global using static LanguageExt.Parsec.Prim;
global using PNixExpr = (
    DotNix.Parsing.NixExpr Expr,
    LanguageExt.Parsec.Pos BeginPos,
    LanguageExt.Parsec.Pos EndPos,
    int BeginIndex,
    int EndIndex
);