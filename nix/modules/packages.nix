{ lib, ... }:
with lib;
let
  overlay =
    final: prev:
    let
      packages = packagesFromDirectoryRecursive {
        callPackage = callPackageWith (final // packages);
        directory = ../pkgs;
      };
    in
    packages;
in
{
  perSystem =
    { pkgs, ... }:
    {
      legacyPackages = overlay pkgs pkgs;
    };
}
