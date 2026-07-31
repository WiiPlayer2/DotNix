using System.Text;
using DotNix.Parsing.Models;
using LanguageExt;
using TreeSitter;
using static LanguageExt.Prelude;

namespace DotNix.Parsing;

public static class NixParser
{
    private static readonly Parser parser;

    static NixParser()
    {
        var language = new Language("/nix/store/9q1gf3k335d8q445nwy1w8ma25aqpsll-tree-sitter-nix-0.3.0-unstable-2025-12-03/parser", "tree_sitter_nix");
        parser = new Parser(language);
    }
    
    public static ParserResult<NixExpression> Parse(string code, CancellationToken cancellationToken)
    {
        using var tree = parser.Parse(code);
        if (tree is null)
            return ParserResult.Error(new ParserError("no tree"));
        return MapNode(tree.RootNode);
    }
    
    #region Mappers

    private static NixExpression MapNode(Node node) => node.Type switch
    {
        "source_code" => MapNode(node.GetField("expression")),
        "variable_expression" => NixExpression.Variable(node.GetField("name").Text),
        "integer_expression" => NixExpression.Integer(long.Parse(node.Text)),
        "float_expression" => NixExpression.Float(double.Parse(node.Text)),
        "string_expression" => MapStringExpression(node),
        "indented_string_expression" => MapIndentedStringExpression(node),
        "path_expression" => MapPathExpression(node),
        "parenthesized_expression" => MapNode(node.GetField("expression")),
        "attrset_expression" => MapAttrs(node),
        "rec_attrset_expression" => MapRecAttrs(node),
        "let_attrset_expression" => MapLetAttrs(node),
        "list_expression" => MapList(node),
        "select_expression" => MapSelect(node),
        _ => throw new NotImplementedException($"Type {node.Type} not implemented"),
    };

    private static NixExpression MapSelect(Node node) => NixExpression.Select(MapNode(node.GetField("expression")), MapAttrPath(node.GetField("attrpath")), node.TryGetField("default").Map(MapNode));

    private static NixExpression MapList(Node node) => NixExpression.List(toList(node.GetFields("element").Select(MapNode)));

    private static NixExpression MapAttrs(Node node) => NixExpression.Attrs(MapBindingSet(node.FirstNamedChild!));
    
    private static NixExpression MapRecAttrs(Node node) => NixExpression.RecAttrs(MapBindingSet(node.FirstNamedChild!));
    
    private static NixExpression MapLetAttrs(Node node) => NixExpression.LetAttrs(MapBindingSet(node.FirstNamedChild!));

    private static Lst<NixBinding> MapBindingSet(Node node) => toList(node.GetFields("binding").Select(MapBinding));
    
    private static NixBinding MapBinding(Node node) => new(MapAttrPath(node.GetField("attrpath")), MapNode(node.GetField("expression")));

    private static NixAttrPath MapAttrPath(Node node) => new(toList(node.GetFields("attr").Select(MapAttrPathSegment)));

    private static NixAttrPathSegment MapAttrPathSegment(Node node) => node.Type switch
    {
        "identifier" => NixAttrPathSegment.Identifier(node.Text),
        "string_expression" => NixAttrPathSegment.Interpolation(MapStringExpression(node)),
        "interpolation" => NixAttrPathSegment.Interpolation(MapNode(node.GetField("expression"))),
        _ => throw new NotImplementedException($"Type {node.Type} not implemented"),
    };
    
    private static NixExpression MapStringExpression(Node node) => NixExpression.String(
        toList(CleanupFragments(
            node.Fields
                .Skip(1)
                .Take(node.Fields.Count - 2)
                .Select(kv => MapStringFragment(kv.Value))
        ))
    );

    private static NixExpression MapPathExpression(Node node) => NixExpression.Path(
        toList(CleanupFragments(
            node.Fields
                .Select(kv => MapPathFragment(kv.Value))
        ))
    );

    private static NixExpression MapIndentedStringExpression(Node node)
    {
        var rawFragments = node.Fields
            .Skip(1)
            .Take(node.Fields.Count - 2)
            .Select(kv => MapIndentedStringFragment(kv.Value));
        var cleanedUpFragments = toList(CleanupFragments(rawFragments));
        // TODO: use correct indent
        var fragments = cleanedUpFragments
            .SetItem(0, cleanedUpFragments[0] is NixStringFragment.Text_ text ? NixStringFragment.Text(text.Value.TrimStart('\n')) : cleanedUpFragments[0]);
        return NixExpression.String(toList(fragments));
    }

    private static NixStringFragment MapStringFragment(Node node) => node.Type switch
    {
        "string_fragment" => NixStringFragment.Text(node.Text),
        "interpolation" => NixStringFragment.Interpolation(MapNode(node.GetField("expression"))),
        "dollar_escape" => NixStringFragment.Text(string.Empty),
        "escape_sequence" => NixStringFragment.Text(node.Text switch
        {
            "\\\"" => "\"",
            _ => throw new NotImplementedException($"Escape sequence {node.Text} not implemented"),
        }),
        _ => throw new NotImplementedException($"Type {node.Type} not implemented"),
    };

    private static NixStringFragment MapPathFragment(Node node) => node.Type switch
    {
        "path_fragment" => NixStringFragment.Text(node.Text),
        "interpolation" => NixStringFragment.Interpolation(MapNode(node.GetField("expression"))),
        _ => throw new NotImplementedException($"Type {node.Type} not implemented"),
    };

    private static IEnumerable<NixStringFragment> CleanupFragments(IEnumerable<NixStringFragment> fragments)
    {
        StringBuilder? stringBuilder = null;
        foreach (var fragment in fragments)
        {
            if (fragment is NixStringFragment.Text_ text)
            {
                stringBuilder ??= new();
                stringBuilder.Append(text.Value);
            }
            else
            {
                if (stringBuilder is not null)
                {
                    yield return NixStringFragment.Text(stringBuilder.ToString());
                    stringBuilder = null;
                }

                yield return fragment;
            }
        }

        if (stringBuilder is null) yield break;
        
        yield return NixStringFragment.Text(stringBuilder.ToString());
    }
    
    private static NixStringFragment MapIndentedStringFragment(Node node) => node.Type switch
    {
        "string_fragment" => NixStringFragment.Text(node.Text),
        "interpolation" => NixStringFragment.Interpolation(MapNode(node.GetField("expression"))),
        "dollar_escape" => NixStringFragment.Text(string.Empty),
        "escape_sequence" => NixStringFragment.Text(node.Text switch
        {
            "'''" => "''",
            _ => throw new NotImplementedException($"Escape sequence {node.Text} not implemented"),
        }),
        _ => throw new NotImplementedException($"Type {node.Type} not implemented"),
    };

    #endregion
}