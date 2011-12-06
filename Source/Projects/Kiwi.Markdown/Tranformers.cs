using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorCode;

namespace Kiwi.Markdown
{
	public class Tranformers : ITranformers
	{
		private const string CodeBlockMarker = "```";

		private readonly CodeColorizer _syntaxHighlighter;

		private Regex _cSharpCodeBlocksRegExPreTrans;
		private Regex _jsCodeBlocksRegExPreTrans;
		private Regex _htmlCodeBlocksRegExPreTrans;
		private Regex _cssCodeBlocksRegExPreTrans;
		private Regex _xmlCodeBlocksRegExPreTrans;
		private Regex _genericCodeBlocksRegExPreTrans;

		public Func<string, string> LineBreaks { get; set; }

		public Func<string, string> HtmlEncoding { get; set; }

		public Func<string, string> GenericCodeBlock { get; set; }

		public Func<string, string> CSharp { get; set; }

		public Func<string, string> JavaScript { get; set; }

		public Func<string, string> Html { get; set; }

		public Func<string, string> Css { get; set; }

		public Func<string, string> Xml { get; set; }

		public Tranformers()
		{
			_syntaxHighlighter = new CodeColorizer();

			InitializeTransformers();
		}

		private void InitializeTransformers()
		{
			OnInitializeTranformerRegExs();
			OnInitializeTranformerFuncs();
		}

		protected virtual void OnInitializeTranformerRegExs()
		{
			const string format = @"^{0}([\s]*){1}(.*?){0}";

			_cSharpCodeBlocksRegExPreTrans = new Regex(format.Apply(CodeBlockMarker, "(c#|csharp){1}"), RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_jsCodeBlocksRegExPreTrans = new Regex(format.Apply(CodeBlockMarker, "(js|javascript){1}"), RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_htmlCodeBlocksRegExPreTrans = new Regex(format.Apply(CodeBlockMarker, "html"), RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_cssCodeBlocksRegExPreTrans = new Regex(format.Apply(CodeBlockMarker, "css"), RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_xmlCodeBlocksRegExPreTrans = new Regex(format.Apply(CodeBlockMarker, "xml"), RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_genericCodeBlocksRegExPreTrans = new Regex(format.Apply(CodeBlockMarker, string.Empty), RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		protected virtual void OnInitializeTranformerFuncs()
		{
			LineBreaks = mc => mc.Replace("\r\n", "\n");

			HtmlEncoding = mc => mc.Replace(@"\<", "&lt;").Replace(@"\>", "&gt;");

			CSharp = mc => _cSharpCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.CSharp));

			JavaScript = mc => _jsCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.JavaScript));

			Html = mc => _htmlCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.Html));

			Css = mc => _cssCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.Css));

			Xml = mc => _xmlCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.Xml));

			GenericCodeBlock = mc => _genericCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value));
		}

		protected virtual string FormatAndColorize(string value, ILanguage language = null)
		{
			var output = new StringBuilder();
			foreach (var line in value.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(s => !s.StartsWith(CodeBlockMarker)))
			{
				if(language == null)
					output.Append(new string(' ', 4));

				output.AppendLine(line);
			}

			return language != null
				? _syntaxHighlighter.Colorize(output.ToString().Trim(), language)
				: output.ToString();
		}

		public virtual IEnumerable<Func<string, string>> GetTransformers()
		{
			yield return LineBreaks;

			yield return CSharp;

			yield return JavaScript;

			yield return Html;

			yield return Css;

			yield return Xml;

			yield return GenericCodeBlock;

			yield return HtmlEncoding;
		}
	}
}