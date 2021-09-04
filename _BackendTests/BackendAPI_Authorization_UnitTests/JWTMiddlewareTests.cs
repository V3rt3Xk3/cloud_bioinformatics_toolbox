using System.Threading.Tasks;

using Backend.Authorization;
using Backend.Models;

using Xunit;
using Xunit.Extensions.Ordering;
using BackendTests.Utilities;

namespace BackendTests.UnitTests
{
	[Collection("JWTUnitTests")]
	public class JWTMiddleWareTests
	{
		private IJWTUtils _jwtUtils;
		public JWTMiddleWareTests(IJWTUtils jwtUtils)
		{
			this._jwtUtils = jwtUtils;
		}

		[Fact]
		public void TC0001_GenerateAccessToken()
		{
			// Arange
			UserEntity user = new();
			user.Id = "000000000000000000000001";

			// Act
			string accessToken = _jwtUtils.GenerateAccessToken(user);

			// Assert
			string errorMessage = "The accessToken generated seems to equal Null";
			AssertX.NotEqual(null, accessToken, errorMessage);
		}
	}
}