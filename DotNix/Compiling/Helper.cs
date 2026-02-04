using System;
using DotNix.Types;

namespace DotNix.Compiling;

public static class Helper
{
    public static NixValueThunked Thunk(Func<NixValueThunked> fn) => Thunk(() => Task.FromResult(fn()));

    public static NixValueThunked Thunk(Func<Task<NixValueThunked>> fn) => NixValueThunked.Thunk(new(fn));
}
