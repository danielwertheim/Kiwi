using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColorCode;

namespace Kiwi.Transformations
{
	public class Tranformers : ITranformers
	{
		private const string CodeBlockMarker = "```";

		private readonly CodeColorizer _syntaxHighlighter;

		private Regex _cSharpCodeBlocksRegExPreTrans;
		private Regex _jsCodeBlocksRegExPreTrans;
		private Regex _htmlCodeBlocksRegExPreTrans;
		private Regex _cssCodeBlocksRegExPreTrans;
		private Regex _genericCodeBlocksRegExPreTrans;

		public Func<string, string> PreGeneric { get; set; }

		public Func<string, string> PostGeneric { get; set; }

		public Func<string, string> CSharp { get; set; }

		public Func<string, string> JavaScript { get; set; }

		public Func<string, string> Html { get; set; }

		public Func<string, string> Css { get; set; }

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
			_cSharpCodeBlocksRegExPreTrans = new Regex(@"^```c#(.*?)```", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_jsCodeBlocksRegExPreTrans = new Regex(@"^```javascript(.*?)```", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_htmlCodeBlocksRegExPreTrans = new Regex(@"^```html(.*?)```", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
			_cssCodeBlocksRegExPreTrans = new Regex(@"^```css(.*?)```", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

			_genericCodeBlocksRegExPreTrans = new Regex(@"^```(.*?)```", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		protected virtual void OnInitializeTranformerFuncs()
		{
			CSharp = mc => _cSharpCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.CSharp));

			JavaScript = mc => _jsCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.JavaScript));

			Html = mc => _htmlCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.Html));

			Css = mc => _cssCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value, Languages.Css));

			PreGeneric = mc => _genericCodeBlocksRegExPreTrans.Replace(mc, m => FormatAndColorize(m.Value));

			PostGeneric = mc => mc.Replace("\n", "\r\n");
		}

		protected virtual string FormatAndColorize(string value, ILanguage language = null)
		{
			var output = new StringBuilder();
			foreach (var line in value.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(s => !s.StartsWith(CodeBlockMarker)))
			{
				if(language == null)
					output.Append(new string(' ', 4));

				output.AppendLine(line);
			}

			return language != null
				? _syntaxHighlighter.Colorize(output.ToString().Trim(), language)
				: output.ToString();
		}

		public virtual IEnumerable<Func<string, string>> GetPreTransformers()
		{
			yield return CSharp;

			yield return JavaScript;

			yield return Html;

			yield return Css;

			yield return PreGeneric;
		}

		public virtual IEnumerable<Func<string, string>> GetPostTransformers()
		{
			yield return PostGeneric;
		}
	}
}