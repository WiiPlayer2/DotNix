using System;

namespace DotNix.Types;

public record NixBool : NixValueStrict
{
    public static NixBool True => field ??= new(true);
    
    public static NixBool False => field ??= new(false);
    
    private NixBool(bool value)
    {
        Value = value;
    }
    
    public bool Value { get; }
    
    public static implicit operator NixBool(bool value) => value ? True : False;
}
