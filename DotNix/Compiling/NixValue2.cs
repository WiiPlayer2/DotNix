namespace DotNix.Compiling;

public abstract record NixValue2
{
    public NixValue2()
    {
        UnThunk = Strict = new(() => Task.FromResult(this));
    }
    
    public virtual AsyncLazy<NixValue2> Strict { get; }
    
    public virtual AsyncLazy<NixValue2> UnThunk { get; }
}