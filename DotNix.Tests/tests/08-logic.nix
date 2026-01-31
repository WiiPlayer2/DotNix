with builtins;
let
    _booleans = [ true false ];
in
let
    _table = op: concatMap (a: map (op a) _booleans) _booleans;
 
    _add = a: b: a && b;
    _or = a: b: a || b;
    _impl = a: b: a -> b;
in
{
    add = _table _add;
    or = _table _or;
    impl = _table _impl;
    not = map (a: !a) _booleans;
}