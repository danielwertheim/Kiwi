using System;
using System.Collections.Generic;

namespace Kiwi.Markdown
{
	public interface ITranformers
	{
		Func<string, string> PreGeneric { get; set; }
		Func<string, string> PostGeneric { get; set; }

		Func<string, string> CSharp { get; set; }
		Func<string, string> JavaScript { get; set; }
		Func<string, string> Html { get; set; }
		Func<string, string> Css { get; set; }

		IEnumerable<Func<string, string>> GetPreTransformers();

		IEnumerable<Func<string, string>> GetPostTransformers();
	}
}