namespace DotNix.Compiling;

public abstract record NixValue2
{
    public NixValue2()
    {
        Strict = new(() => Task.FromResult(this));
    }
    
    public virtual AsyncLazy<NixValue2> Strict { get; }
}