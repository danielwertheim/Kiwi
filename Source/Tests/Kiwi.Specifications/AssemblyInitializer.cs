using Machine.Specifications;

namespace Kiwi.Specifications
{
	public class AssemblyInitializer : IAssemblyContext
	{
		private static bool _isInitialized;

		public void OnAssemblyStart()
		{
			if (_isInitialized)
				return;

			_isInitialized = true;
		}

		public void OnAssemblyComplete()
		{
		}
	}
}