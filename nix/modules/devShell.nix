{
  perSystem =
    { pkgs, self', ... }:
    {
      devShells.default = pkgs.mkShell {
        packages = with pkgs; with self'.legacyPackages; [
          parser-gen
        ];
      };
    };
}
