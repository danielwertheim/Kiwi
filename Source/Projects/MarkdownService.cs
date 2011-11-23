using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorCode;
using MarkdownSharp;

namespace Kiwi
{
    public class MarkdownService : IMarkdownService
    {
        private readonly string _directoryPath;
        private readonly Markdown _markdown;
        private readonly TextInfo _invariantTextInfo;
        private readonly Regex _cSharpCodeBlocksRegExPreTrans;
        private readonly Regex _jsCodeBlocksRegExPreTrans;
        private readonly CodeColorizer _syntaxHighlighter;

        public MarkdownService(string directoryPath)
        {
            _directoryPath = directoryPath;

            _markdown = new Markdown(new MarkdownOptions());
            _syntaxHighlighter = new CodeColorizer();
            _invariantTextInfo = CultureInfo.InvariantCulture.TextInfo;

            _cSharpCodeBlocksRegExPreTrans = new Regex(@"^```c#(.*?)```", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _jsCodeBlocksRegExPreTrans = new Regex(@"^```javascript(.*?)```", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public Document GetDocument(string docId)
        {
            return new Document
            {
                Title = _invariantTextInfo.ToTitleCase(docId.Replace("-", string.Empty)),
                TransformedMarkdown = ApplyTransformation(File.ReadAllText(GetFilePath(docId)))
            };
        }

        private string GetFilePath(string docId)
        {
            return Path.Combine(_directoryPath, docId + ".md");
        }

        private string ApplyTransformation(string markdownContent)
        {
            foreach (var preTransformation in GetPreTransformations())
                markdownContent = preTransformation.Invoke(markdownContent);

            return _markdown.Transform(markdownContent);
        }

        private IEnumerable<Func<string, string>> GetPreTransformations()
        {
            yield return mc => _cSharpCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.CSharp));

            yield return mc => _jsCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.JavaScript));
        }

        private string FormatAndColorize(string value, ILanguage language)
        {
            var tmp = new StringBuilder();

            foreach (var s in value.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(s => !s.StartsWith("```")))
                tmp.AppendLine(s);

            return _syntaxHighlighter.Colorize(tmp.ToString(), language);
        }
    }
}