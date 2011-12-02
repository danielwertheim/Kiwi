using System;

namespace Kiwi.Markdown
{
    [Serializable]
    public class Document
    {
        public string Title { get; set; }

        public string Content { get; set; }
    }
}