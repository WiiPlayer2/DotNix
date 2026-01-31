using System;

namespace DotNix.Types;

public record NixString(string Value) : NixValueStrict;
