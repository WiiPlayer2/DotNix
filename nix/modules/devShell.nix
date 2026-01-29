{
  perSystem =
    { pkgs, self', ... }:
    {
      devShells.default = pkgs.mkShell {
        name = "dotnix";

        packages = with pkgs; with self'.legacyPackages; [
          parser-gen
          generate-parser
        ];
      };
    };
}
