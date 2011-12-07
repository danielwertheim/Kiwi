namespace Kiwi.Markdown
{
    public interface IMarkdownService
    {
        Document GetDocument(string docId);

    	string ToHtml(string markdown);
    }
}