with builtins;
let
    _values = [ (-1) 0 1 (-1.0) 0.0 1.0 ];
    _table = op: concatMap (a: map (op a) _values) _values;
    
    _ops = {
        _eq = a: b: a == b;
        _ne = a: b: a != b;
        _lt = a: b: a < b;
        _le = a: b: a <= b;
        _gt = a: b: a > b;
        _ge = a: b: a >= b;
    };
in
mapAttrs (_: _table) _ops