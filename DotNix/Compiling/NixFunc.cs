namespace DotNix.Compiling;

public delegate Task<NixValueThunked> NixFunc(NixValueThunked arg);