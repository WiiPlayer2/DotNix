let
    _a = {
        a = [ 0 1 2 ];
    };
in
_a // {
    a = _a.a ++ [ 3 4 5 ];
    b = [ true false ];
}