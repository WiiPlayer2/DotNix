{
  perSystem =
    { pkgs, self', ... }:
    {
      devShells.default = pkgs.mkShell {
        name = "dotnix";

        packages = with pkgs; [
          dotnet-sdk_10
          omnisharp-roslyn
        ];
      };
    };
}
