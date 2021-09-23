using System;

using BackendTests.Utilities;

using Xunit;
using Xunit.Extensions.Ordering;

namespace BackendTests.UnitTestUtilitiesTests
{
	[Collection("UnitTestUtilitiesTests")]
	public class CustomAsserts_UnitTests
	{


		[Theory]
		[InlineData(1, 1)]
		[InlineData(17.98, 17.98)]
		[InlineData("610ed5dcea16532c2368ca8d", "610ed5dcea16532c2368ca8d")]
		public void TC0001_AssertXEqualTests<T>(T a, T b)
		{
			string errorMessage = "The input 'a' and input 'b' are not equal!";
			AssertX.Equal(a, b, errorMessage);
		}
		[Theory]
		[InlineData(13, 1)]
		[InlineData(17.986, 17.98)]
		[InlineData("610ed5dcea16532c2368ca8s", "610ed5dcea16532c2368ca8d")]
		public void TC0002_AssertXNotEqualTests<T>(T a, T b)
		{
			string errorMessage = "The input 'a' and input 'b' ARE equal!";
			AssertX.NotEqual(a, b, errorMessage);
		}


		[Fact]
		public void TC0003_01_AssertXInRangeTests_Integers()
		{
			int[][] testInput = new int[3][];
			testInput[0] = new int[3] { 4, 0, 10 };
			testInput[1] = new int[3] { 1, 1, 10 };
			testInput[2] = new int[3] { 10, 4, 10 };

			string errorMessage = "The testInput is out of range (low <= testInput <= high)!";

			for (int i = 0; i < testInput.Length; i++)
			{
				AssertX.InRange(testInput[i][0], testInput[i][1], testInput[i][2], errorMessage);
			}

		}
		[Fact]
		public void TC0003_02_AssertXInRangeTests_Double()
		{
			double[][] testInput = new double[3][];
			testInput[0] = new double[3] { 19.3126, 12.983, 33.123 };
			// NOTE: As we are talking about double, the input==low are a digit off; 
			testInput[1] = new double[3] { 17.324, 17.323, 93.12351 };
			// NOTE: As we are talking about double, the input==high are a digit off;
			testInput[2] = new double[3] { 39.94, 4.124, 39.95 };

			string errorMessage = "The testInput is out of range (low <= testInput <= high)!";

			for (int i = 0; i < testInput.Length; i++)
			{
				AssertX.InRange(testInput[i][0], testInput[i][1], testInput[i][2], errorMessage);
			}

		}
		[Fact]
		public void TC0003_03_AssertXInRangeTests_String()
		{
			string[][] testInput = new string[3][];
			testInput[0] = new string[3] { "c", "a", "g" };
			// NOTE: As we are talking about double, the input==low are a digit off; 
			testInput[1] = new string[3] { "a", "a", "w" };
			// NOTE: As we are talking about double, the input==high are a digit off;
			testInput[2] = new string[3] { "y", "k", "y" };

			string errorMessage = "The testInput is out of range (low <= testInput <= high)!";

			for (int i = 0; i < testInput.Length; i++)
			{
				AssertX.InRange(testInput[i][0], testInput[i][1], testInput[i][2], errorMessage);
			}

		}
		[Fact]
		public void TC0003_04_AssertXInRangeTests_DateTime()
		{
			DateTime now = DateTime.UtcNow;
			DateTime[][] testInput = new DateTime[3][];
			testInput[0] = new DateTime[3] { now, now.AddMinutes(-10), now.AddMinutes(+10) };
			// NOTE: As we are talking about double, the input==low are a digit off; 
			testInput[1] = new DateTime[3] { now, now, now.AddMinutes(+10) };
			// NOTE: As we are talking about double, the input==high are a digit off;
			testInput[2] = new DateTime[3] { now, now.AddMinutes(-10), now };

			string errorMessage = "The testInput is out of range (low <= testInput <= high)!";

			for (int i = 0; i < testInput.Length; i++)
			{
				AssertX.InRange(testInput[i][0], testInput[i][1], testInput[i][2], errorMessage);
			}

		}
	}
}