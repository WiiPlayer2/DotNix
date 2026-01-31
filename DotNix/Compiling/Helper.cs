using System;

namespace DotNix.Compiling;

public static class Helper
{
    public static NixThunk Thunk(Func<NixValue2> fn) => Thunk(() => Task.FromResult(fn()));

    public static NixThunk Thunk(Func<Task<NixValue2>> fn) => new(new(fn));
}
