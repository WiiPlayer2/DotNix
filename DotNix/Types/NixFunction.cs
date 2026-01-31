using System;

namespace DotNix.Types;

public record NixFunction(NixFunc Fn) : NixValueStrict;