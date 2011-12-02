using Kiwi.Transformations;
using Kiwi.Transformations.ContentProviders;
using Machine.Specifications;

namespace Kiwi.Specifications.Transformations
{
	public static class ComplecteSpecificationHtml
	{
		public readonly static string Html;
	
		static ComplecteSpecificationHtml()
		{
			Html = System.IO.File.ReadAllText(@"Transformations\Complete-Specification.html");
		}
	}

	public class CompleteSpecification : SpecificationBase
	{
		Establish context = () =>
		{
			_markdownService = new MarkdownService(new FileContentProvider("Transformations"));
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
		private static Document _document;
	}
}