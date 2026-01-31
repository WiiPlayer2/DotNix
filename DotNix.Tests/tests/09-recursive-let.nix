with builtins;
let
    _list1 = [ 1 2 3 ];
    _list2 = map (a: [ a ]) _list1;
in
_list2