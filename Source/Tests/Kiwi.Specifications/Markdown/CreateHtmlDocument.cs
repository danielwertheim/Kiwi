using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;
using Machine.Specifications;

namespace Kiwi.Specifications.Markdown
{
	public static class ComplecteSpecificationHtml
	{
		public readonly static string Html;
	
		static ComplecteSpecificationHtml()
		{
			Html = System.IO.File.ReadAllText(@"Markdown\Complete-Specification.html");
		}
	}

	public class when_creating_html_document_from_markdown_file : SpecificationBase
	{
		Establish context = () =>
		{
			_markdownService = new MarkdownService(new FileContentProvider("Markdown"));
		};

		Because of = 
			() => _document = _markdownService.GetDocument("Complete-Specification");

		It should_have_created_a_document = 
			() => _document.ShouldNotBeNull();

		It should_have_created_a_document_with_formatted_title =
			() => _document.Title.ShouldEqual("Complete Specification");

		It should_have_created_a_document_with_transformed_content = 
			() => _document.Content.ShouldEqual(ComplecteSpecificationHtml.Html);
		
		private static IMarkdownService _markdownService;
		private static HtmlDocument _document;
	}
}