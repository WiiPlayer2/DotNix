using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using Char = LanguageExt.Parsec.Char;

namespace DotNix.Utils.TreeSitter;

public class TreeSitterParser
{
    private readonly Parser<TreeSitterNode> parser;

    private TreeSitterParser(Parser<TreeSitterNode> parser)
    {
        this.parser = parser;
    }
    
    public static async Task<TreeSitterParser> Create(Stream grammarJson, Stream nodeTypesJson,
        CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            RespectNullableAnnotations = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        };
        options.Converters.Add(new LstJsonConverter());
        var grammarDto = await JsonSerializer.DeserializeAsync<TreeSitterGrammarDto>(grammarJson, options, cancellationToken: cancellationToken);
        var nodeTypesDto = await JsonSerializer.DeserializeAsync<TreeSitterNodeTypeDto[]>(nodeTypesJson, options, cancellationToken: cancellationToken);
        var rules = BuildRules(grammarDto!.Rules);
        return new(rules[grammarDto.Supertypes.First()]);
    }

    public ParserResult<TreeSitterNode> Parse(string code, CancellationToken cancellationToken) => parse(parser, code);

    private static IReadOnlyDictionary<string, Parser<TreeSitterNode>> BuildRules(IReadOnlyDictionary<string, TreeSitterGrammarRuleDto> rules)
    {
        var parsers = new Dictionary<string, Parser<TreeSitterNode>>()
        {
            ["_path_start"] = failure<TreeSitterNode>("TODO _path_start"),
        };
        foreach (var (name, rule) in rules)
        {
            parsers[name] = BuildRule(rule);
        }

        return parsers;

        Parser<TreeSitterNode> BuildRule(TreeSitterGrammarRuleDto rule)
        {
            var p = attempt(BuildRule2(rule));
            return input =>
            {
                var result = p(input);
                Debug.WriteLine($"[{(result.IsFaulted ? "✘" : "✓")}, {result.Tag,8}, {result.Reply.State?.Index, 4}, {rule, -80}] {result.Reply.Error?.ToString() ?? result.Reply.Result?.ToString()}");
                return result;
            };
        }
        
        Parser<TreeSitterNode> BuildRule2(TreeSitterGrammarRuleDto rule) => rule switch
        {
            TreeSitterGrammarRuleDto.Token token => BuildRule(token.Content),
            TreeSitterGrammarRuleDto.Repeat1 repeat1 => many1(BuildRule(repeat1.Content)).Map(x => default(TreeSitterNode)),
            TreeSitterGrammarRuleDto.ImmediateToken immediateToken => BuildRule(immediateToken.Content),
            TreeSitterGrammarRuleDto.PrecRight precRight => BuildRule(precRight.Content),
            TreeSitterGrammarRuleDto.PrecLeft precLeft => BuildRule(precLeft.Content),
            TreeSitterGrammarRuleDto.Alias alias => BuildRule(alias.Content).label(alias.Value),
            TreeSitterGrammarRuleDto.Prec prec => BuildRule(prec.Content),
            TreeSitterGrammarRuleDto.Blank blank => unitp.Map(x => default(TreeSitterNode)),
            TreeSitterGrammarRuleDto.Repeat repeat => many(BuildRule(repeat.Content)).Map(x => default(TreeSitterNode)),
            TreeSitterGrammarRuleDto.String @string => str(@string.Value).Map(x => default(TreeSitterNode)),
            TreeSitterGrammarRuleDto.Pattern pattern =>
                from input in Input()
                from pos in getIndex
                let regex = new Regex(pattern.Value)
                let match = regex.Match(input, pos)
                let isMatch = match.Success && match.Index == pos
                from value in isMatch ? asString(count(match.Length, anyChar)) : failure<string>($"Pattern /{pattern.Value}/ doesn't match")
                from newPos in getIndex
                let _dbg = new{match,isMatch,value,pos,newPos}
                select default(TreeSitterNode),
            TreeSitterGrammarRuleDto.Field field => BuildRule(field.Content),
            TreeSitterGrammarRuleDto.Seq seq => chain(seq.Members.Select(BuildRule).ToArray()).Map(x => default(TreeSitterNode)),
            TreeSitterGrammarRuleDto.Choice choice => Prim.choice(choice.Members.Select(x => attempt(BuildRule(x))).Reverse().ToArray()),
            TreeSitterGrammarRuleDto.Symbol symbol => lazyp(() => parsers[symbol.Name]),
            _ => throw new NotSupportedException($"{rule.GetType().Name} is not supported"),
        };

        Parser<string> Input() => (input => ParserResult.EmptyOK<string>(input.Value, input));
    }
}