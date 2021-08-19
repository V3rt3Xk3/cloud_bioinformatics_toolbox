using System;
using System.Globalization;

namespace Backend.Helpers
{
	// Application specific exceptions
	public class AddException : Exception
	{
		public AddException() : base() { }
		public AddException(string message) : base(message) { }
		public AddException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
		{ }
	}
}