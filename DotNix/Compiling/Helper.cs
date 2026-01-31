using System;
using DotNix.Types;

namespace DotNix.Compiling;

public static class Helper
{
    public static NixThunk Thunk(Func<NixValueThunked> fn) => Thunk(() => Task.FromResult(fn()));

    public static NixThunk Thunk(Func<Task<NixValueThunked>> fn) => new(new(fn));
}
