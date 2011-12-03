using System;
using System.Collections.Generic;

namespace Kiwi.Markdown
{
	public interface ITranformers
	{
		Func<string, string> LineBreaks { get; set; }
		Func<string, string> GenericCodeBlock { get; set; }

		Func<string, string> CSharp { get; set; }
		Func<string, string> JavaScript { get; set; }
		Func<string, string> Html { get; set; }
		Func<string, string> Css { get; set; }
		Func<string, string> Xml { get; set; }

		IEnumerable<Func<string, string>> GetTransformers();
	}
}