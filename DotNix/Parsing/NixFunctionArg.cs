using System;
using FunicularSwitch.Generators;

namespace DotNix.Parsing;

[UnionType]
public abstract partial record NixFunctionArg
{
    public record Simple_(NixIdentifier Name) : NixFunctionArg;
}
