using System;

namespace ZenFulcrum.EmbeddedBrowser
{

	public class ExceptionEventArgs : EventArgs
	{
		public Exception Exception { get; private set; }

		internal ExceptionEventArgs(Exception exception)
		{
			Exception = exception;
		}
	}
}