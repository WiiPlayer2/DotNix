using System;

namespace DotNix.Parsing.Models;

public record NixBinding(NixAttrPath AttrPath, NixExpression Expression);