using System;

namespace Kiwi
{
    [Serializable]
    public class Document
    {
        public string Title { get; set; }

        public string TransformedMarkdown { get; set; }
    }
}