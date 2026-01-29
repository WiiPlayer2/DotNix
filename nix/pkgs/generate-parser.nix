{
  fetchurl,
  writeShellApplication,

  parser-gen,
}:
let
  grammarJson = fetchurl {
    url = "https://raw.githubusercontent.com/nix-community/tree-sitter-nix/refs/heads/master/src/grammar.json";
    hash = "sha256-aKaoCJjvCttatlXgW5kc5dijcY9oN7x3Pw6jBsOkFPY=";
  };
in
writeShellApplication {
  name = "generate-parser";
  runtimeInputs = [
    parser-gen
  ];
  text = ''
    parser-gen generate ${grammarJson} "$@"
  '';

  passthru = {
    inherit grammarJson;
  };
}
