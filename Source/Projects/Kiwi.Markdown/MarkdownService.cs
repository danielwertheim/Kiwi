using System.Globalization;
using MarkdownSharp;

namespace Kiwi.Markdown
{
	public class MarkdownService : IMarkdownService
    {
        private readonly MarkdownSharp.Markdown _markdown;
        private readonly TextInfo _invariantTextInfo;

		public ITranformers Tranformers { get; set; }

		public IContentProvider ContentProvider { get; set; }

		public MarkdownService(IContentProvider contentProvider)
        {
            ContentProvider = contentProvider;

            _markdown = new MarkdownSharp.Markdown(CreateMarkdownOptions());

            _invariantTextInfo = CultureInfo.InvariantCulture.TextInfo;

			Tranformers = new Tranformers();
        }

		private MarkdownOptions CreateMarkdownOptions()
		{
			return new MarkdownOptions
			{
				AutoHyperlink = true,
				AutoNewLines = false,
				EncodeProblemUrlCharacters = true,
				LinkEmails = true
			};
		}

        public virtual Document GetDocument(string docId)
        {
            return new Document
            {
                Title = _invariantTextInfo.ToTitleCase(docId.Replace("-", " ")),
                Content = ApplyTransformation(ContentProvider.GetContent(docId))
            };
        }

        protected virtual string ApplyTransformation(string markdownContent)
        {
            foreach (var preTransformation in Tranformers.GetPreTransformers())
                markdownContent = preTransformation.Invoke(markdownContent);

			var transformed = _markdown.Transform(markdownContent);

        	foreach (var postTransformer in Tranformers.GetPostTransformers())
        		transformed = postTransformer.Invoke(transformed);

        	return transformed;
        }
    }
}