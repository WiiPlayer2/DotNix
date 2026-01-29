using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;

namespace ParserGen;

internal class RuleWriter(IndentedTextWriter writer)
{
    private const string MapToObject = ".Map(x => (object?)x)";

    public async Task WriteRule(RuleDto rule, bool immediate)
    {
        if (rule is RuleDto.ImmediateToken_)
            immediate = true;
        
        if (!immediate)
            await writer.WriteAsync("lexeme(");
        
        await rule.Match(
            WriteAliasRule,
            WriteBlankRule,
            WriteChoiceRule,
            WriteFieldRule,
            WriteImmediateTokenRule,
            WritePatternRule,
            WritePrecRule,
            WritePrecLeftRule,
            WritePrecRightRule,
            WriteRepeatRule,
            WriteRepeat1Rule,
            WriteSeqRule,
            WriteStringRule,
            WriteSymbolRule,
            WriteTokenRule
        );
        
        if (!immediate)
            await writer.WriteAsync(")");
    }

    private async Task WriteSeqRule(RuleDto.Seq_ seqRule)
    {
        await writer.WriteLineAsync("chain((Parser<object?>[])[");
        using (writer.WithIndent())
        {
            var first = true;
            foreach (var member in seqRule.Members)
            {
                await WriteRule(member, first);
                await writer.WriteLineAsync(",");
                first = false;
            }
        }
        await writer.WriteAsync($"]){MapToObject}");
    }

    private async Task WriteStringRule(RuleDto.String_ stringRule)
    {
        await writer.WriteAsync(
            $"str({SymbolDisplay.FormatLiteral(stringRule.Value, true)}){MapToObject}");
    }

    private async Task WritePatternRule(RuleDto.Pattern_ patternRule)
    {
        await writer.WriteAsync($"regex({SymbolDisplay.FormatLiteral(patternRule.Value, true)}){MapToObject}");
    }

    private async Task WriteSymbolRule(RuleDto.Symbol_ symbolRule)
    {
        await writer.WriteAsync($"lazyp(() => {symbolRule.Name})");
    }

    private async Task WriteFieldRule(RuleDto.Field_ fieldRule)
    {
        await writer.WriteAsync("(");
        await WriteRule(fieldRule.Content, true);
        await writer.WriteAsync($").label({SymbolDisplay.FormatLiteral(fieldRule.Name, true)})");
    }

    private async Task WriteChoiceRule(RuleDto.Choice_ choiceRule)
    {
        await writer.WriteLineAsync("choice((Parser<object?>[])[");
        using (writer.WithIndent())
        {
            foreach (var member in choiceRule.Members)
            {
                await writer.WriteAsync("attempt(");
                await WriteRule(member, true);
                await writer.WriteLineAsync("),");
            }
        }
        await writer.WriteAsync("])");
    }

    private async Task WriteRepeatRule(RuleDto.Repeat_ repeatRule)
    {
        await writer.WriteAsync("many(");
        await WriteRule(repeatRule.Content, false);
        await writer.WriteAsync($"){MapToObject}");
    }

    private async Task WriteRepeat1Rule(RuleDto.Repeat1_ repeat1Rule)
    {
        await writer.WriteAsync("many1(");
        await WriteRule(repeat1Rule.Content, false);
        await writer.WriteAsync($"){MapToObject}");
    }

    private async Task WriteBlankRule(RuleDto.Blank_ _)
    {
        await writer.WriteAsync($"eof{MapToObject}");
    }

    private async Task WriteAliasRule(RuleDto.Alias_ aliasRule)
    {
        if (!aliasRule.Named)
            throw new NotImplementedException();

        await writer.WriteAsync("(");
        await WriteRule(aliasRule.Content, true);
        await writer.WriteAsync($").label({SymbolDisplay.FormatLiteral(aliasRule.Value, true)})");
    }
    
    private Task WriteTokenRule(RuleDto.Token_ tokenRule) => WriteRule(tokenRule.Content, true);
    
    private Task WriteImmediateTokenRule(RuleDto.ImmediateToken_ immediateTokenRule) => WriteRule(immediateTokenRule.Content, true);

    private async Task WritePrecRule(RuleDto.Prec_ precRule)
    {
        await writer.WriteAsync($"/* PREC [{precRule.Value,2}] */ ");
        await WriteRule(precRule.Content, true);
    }
    
    private async Task WritePrecLeftRule(RuleDto.PrecLeft_ precLeftRule)
    {
        await writer.WriteAsync($"/* PREC_LEFT [{precLeftRule.Value,2}] */ ");
        await WriteRule(precLeftRule.Content, true);
    }
    
    private async Task WritePrecRightRule(RuleDto.PrecRight_ precRightRule)
    {
        await writer.WriteAsync($"/* PREC_RIGHT[{precRightRule.Value,2}] */ ");
        await WriteRule(precRightRule.Content, true);
    }
}