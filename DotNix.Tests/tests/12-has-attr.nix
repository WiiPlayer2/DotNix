let
    _subject = {
        a = true;
    };
in
{
    a = _subject ? a;
    b = _subject ? b;
}