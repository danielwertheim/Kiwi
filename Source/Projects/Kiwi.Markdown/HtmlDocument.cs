using System;

namespace Kiwi.Markdown
{
    [Serializable]
    public class HtmlDocument
    {
        public string Title { get; set; }

        public string Content { get; set; }
    }
}