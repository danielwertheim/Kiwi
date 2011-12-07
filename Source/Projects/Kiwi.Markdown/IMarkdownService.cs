namespace Kiwi.Markdown
{
    public interface IMarkdownService
    {
        HtmlDocument GetDocument(string docId);

    	string ToHtml(string markdown);
    }
}