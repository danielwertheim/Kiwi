using System.IO;
using System.Text;

namespace Kiwi.Transformations.ContentProviders
{
	public class FileContentProvider : IContentProvider
	{
		public static readonly Encoding DefaultEncoding = Encoding.GetEncoding(1252);

		private readonly string _directoryPath;
		private readonly Encoding _encoding;

		public FileContentProvider(string directoryPath, Encoding encoding = null)
		{
			_directoryPath = directoryPath;
			_encoding = encoding ?? DefaultEncoding;
		}

		public virtual string GetContent(string docId)
		{
			return File.ReadAllText(GetFilePath(docId), _encoding);
		}

		protected virtual string GetFilePath(string docId)
		{
			return Path.Combine(_directoryPath, docId + ".md");
		}
	}
}