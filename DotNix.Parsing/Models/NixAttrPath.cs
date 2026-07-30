using System;
using LanguageExt;

namespace DotNix.Parsing.Models;

public record NixAttrPath(params Lst<NixAttrPathSegment> Segments);