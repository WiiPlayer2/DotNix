using System;

namespace DotNix.Types;

public delegate Task<NixValueThunked> NixFunc(NixValueThunked arg);