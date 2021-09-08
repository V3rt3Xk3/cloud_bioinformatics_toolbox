using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// WOW: I have managed to find this beauty from: https://stackoverflow.com/questions/42203169/how-to-implement-xunit-descriptive-assert-message
namespace BackendTests.Utilities
{
	public class MyEqualException : Xunit.Sdk.EqualException
	{
		public MyEqualException(object expected, object actual, string userMessage)
			: base(expected, actual)
		{
			UserMessage = userMessage;
		}

		public override string Message =>
			(UserMessage + "\n" + base.Message);
	}
	public class MyNotEqualException : Xunit.Sdk.EqualException
	{
		public MyNotEqualException(object expected, object actual, string userMessage)
			: base(expected, actual)
		{
			UserMessage = userMessage;
		}

		public override string Message =>
			(UserMessage + "\n" + base.Message);
	}
	public class MyNotInRangeException : Xunit.Sdk.NotInRangeException
	{
		public MyNotInRangeException(object expected, object low, object high, string userMessage)
			: base(expected, low, high)
		{
			UserMessage = userMessage;
		}

		public override string Message =>
			(UserMessage + "\n" + base.Message);
	}

	public static class AssertX
	{
		/// <summary>
		/// Verifies that two objects are equal, using a default comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The value to be compared against</param>
		/// <param name="userMessage">Message to show in the error</param>
		/// <exception cref="MyEqualException">Thrown when the objects are not equal</exception>
		public static void Equal<T>(T expected, T actual, string userMessage, bool verbose = false)
		{
			bool areEqual;

			if (expected == null || actual == null)
			{
				// If either null, equal only if both null
				areEqual = (expected == null && actual == null);
			}
			else
			{
				// expected is not null - so safe to call .Equals()
				areEqual = expected.Equals(actual);
			}

			if (!areEqual)
			{
				if (!verbose)
				{
					string stackTraceString = CallerX.AssertX_MethodCaller();
					userMessage += "\n\n" + stackTraceString;
					throw new MyEqualException(expected, actual, userMessage);
				}
				else
				{
					string stackTraceString = CallerX.StackTrace10Lines();
					userMessage += "\n\n" + stackTraceString;
					throw new MyEqualException(expected, actual, userMessage);
				}

			}
		}
		/// <summary>
		/// Verifies that two objects are NOT equal, using a default comparer.
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The value to be compared against</param>
		/// <param name="userMessage">Message to show in the error</param>
		/// <exception cref="MyEqualException">Thrown when the objects ARE equal</exception>
		public static void NotEqual<T>(T expected, T actual, string userMessage, bool verbose = false)
		{
			bool areEqual;

			if (expected == null || actual == null)
			{
				// If either null, equal only if both null
				areEqual = (expected == null && actual == null);
			}
			else
			{
				// expected is not null - so safe to call .Equals()
				areEqual = expected.Equals(actual);
			}

			if (areEqual)
			{
				if (!verbose)
				{
					string stackTraceString = CallerX.AssertX_MethodCaller();
					userMessage += "\n\n" + stackTraceString;
					throw new MyNotEqualException(expected, actual, userMessage);
				}
				else
				{
					string stackTraceString = CallerX.StackTrace10Lines();
					userMessage += "\n\n" + stackTraceString;
					throw new MyNotEqualException(expected, actual, userMessage);
				}

			}
		}
		/// <summary>
		/// Verifies that two objects are in range of "low" and "high", using a default comparer.
		/// <para>Needs the Types to be IComparable </para>
		/// </summary>
		/// <typeparam name="T">The type of the objects to be compared</typeparam>
		/// <param name="inputObject">The input object to Assert</param>
		/// <param name="low">The value to be compared against as lower bound</param>
		/// <param name="high">The value to be compared against as higher bound</param>
		/// <param name="userMessage">Message to show in the error</param>
		/// <exception cref="MyNotInRangeException">Thrown when the inputObject is out of range.</exception>
		public static void InRange<T>(T inputObject, T low, T high, string userMessage, bool verbose = false) where T : IComparable<T>
		{
			bool inRange;

			if (inputObject == null)
			{
				inRange = false;
			}
			else
			{
				// expected is not null - so safe to call Comparison operators
				inRange = 0 <= inputObject.CompareTo(low) && inputObject.CompareTo(high) <= 0;
			}

			if (!inRange)
			{
				if (!verbose)
				{
					string stackTraceString = CallerX.AssertX_MethodCaller();
					userMessage += "\n\n" + stackTraceString;
					throw new MyNotInRangeException(inputObject, low, high, userMessage);
				}
				else
				{
					string stackTraceString = CallerX.StackTrace10Lines();
					userMessage += "\n\n" + stackTraceString;
					throw new MyNotInRangeException(inputObject, low, high, userMessage);
				}

			}
		}
	}
	public static class CallerX
	{
		public static string CallerName([CallerMemberName] string callerMember = "")
		{
			return callerMember;
		}
		public static string StackTrace10Lines()
		{
			string[] lines = Environment.StackTrace.Split('\n');
			string output = "First 5 lines of StackTrace \n";
			for (int i = 0; i < 10; i++)
			{
				output += lines[i] + '\n';
			}
			return output;
		}
		public static string AssertX_MethodCaller()
		{
			string[] lines = Environment.StackTrace.Split('\n');
			string output = "TestCase Failing: \n";
			output += lines[3] + '\n';
			return output;
		}
	}
}
