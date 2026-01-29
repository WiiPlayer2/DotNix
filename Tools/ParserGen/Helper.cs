using System.CodeDom.Compiler;

namespace ParserGen;

public static class Helper
{
    private class ActionDisposable(Action action) : IDisposable
    {
        public void Dispose() => action();
    }

    public static IDisposable Scoped(Action action) => new ActionDisposable(action);

    public static IDisposable WithIndent(this IndentedTextWriter indentedTextWriter)
    {
        indentedTextWriter.Indent++;
        return Scoped(() => indentedTextWriter.Indent--);
    }
}