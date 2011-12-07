using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;
using Machine.Specifications;

namespace Kiwi.Specifications.Markdown
{
	public class when_transforming_markdown_to_html : SpecificationBase
	{
		Establish context = () =>
		{
			_contentProvider = new FileContentProvider("Markdown");
			_markdownService = new MarkdownService(_contentProvider);
		};

		Because of =
			() => _html = _markdownService.ToHtml(_contentProvider.GetContent("Complete-Specification"));

		It should_have_returned_some_html =
			() => _html.ShouldNotBeNull();

		It should_have_created_a_transformed_html_string = 
			() => _html.ShouldEqual(ComplecteSpecificationHtml.Html);
		
		private static IMarkdownService _markdownService;
		private static string _html;
		private static IContentProvider _contentProvider;
	}
}