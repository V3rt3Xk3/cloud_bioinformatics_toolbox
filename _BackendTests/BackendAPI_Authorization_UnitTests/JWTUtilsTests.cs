using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Backend.Authorization;
using Backend.Models;
using Backend.Helpers;

using Xunit;
using Xunit.Extensions.Ordering;
using BackendTests.Utilities;

namespace BackendTests.UnitTests
{
	[Collection("JWTUnitTests")]
	public class JWTUtilsTests
	{
		private IJWTUtils _jwtUtils;
		private AppSettings _appSettings;
		public JWTUtilsTests()
		{
			this._appSettings = new() { Secret = "This is a testing Secret!", RefreshTokenExpiresDuration = 7 };
			IOptions<AppSettings> _appSettingsOptions = Options.Create<AppSettings>(_appSettings);

			this._jwtUtils = new JWTUtils(_appSettingsOptions);
		}

		[Theory]
		[InlineData("610ed5dcea16532c2368ca8d")]
		[InlineData("610ed308fa14d8eb6f5fbb2b")]
		[InlineData("610eeafd4e43d59f6132f979")]
		public void TC0001_GenerateAccessToken(string userID)
		{
			// Arange
			UserEntity user = new();
			user.Id = userID;

			// Act
			string accessToken = _jwtUtils.GenerateAccessToken(user);

			// Assert
			string errorMessage = "The accessToken generated seems to equal Null";
			AssertX.NotEqual(null, accessToken, errorMessage);
		}
		[Theory]
		[InlineData("610ed5dcea16532c2368ca8d")]
		[InlineData("610ed308fa14d8eb6f5fbb2b")]
		[InlineData("610eeafd4e43d59f6132f979")]
		public void TC0002_GenerateAndValidateAccessToken(string userID)
		{
			// Arange
			UserEntity user = new();
			user.Id = userID;

			// Act
			string accessToken = _jwtUtils.GenerateAccessToken(user);
			string validatedUserID = _jwtUtils.ValidateAccessToken(accessToken);

			// Assert
			string errorMessage = "The accessToken gets validated to a different userID than the input.";
			AssertX.Equal(userID, validatedUserID, errorMessage);
		}
		[Theory]
		[InlineData("127.0.0.1")]
		[InlineData("111.97.231.15")]
		[InlineData("196.65.125.9")]
		public void TC0003_GenerateRefreshToken(string ipAddress)
		{
			// Arange / Act
			RefreshToken refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);

			// Assert
			string errorMessage = "The refreshToken seems to return with a null.";
			AssertX.NotEqual(null, refreshToken, errorMessage);
		}
		[Theory]
		[InlineData("127.0.0.1")]
		[InlineData("111.97.231.15")]
		[InlineData("196.65.125.9")]
		public void TC0004_GenerateRefreshTokenAndCheckCreatedByIp(string ipAddress)
		{
			// Arange / Act
			RefreshToken refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);

			// Assert
			string errorMessage = "The refreshToken seems to return with a null.";
			AssertX.Equal(ipAddress, refreshToken.CreatedByIp, errorMessage);
		}
		[Theory]
		[InlineData("127.0.0.1")]
		[InlineData("111.97.231.15")]
		[InlineData("196.65.125.9")]
		public void TC0005_GenerateRefreshTokenAndCheckCreatedDateTime(string ipAddress)
		{
			// Arange / Act
			DateTime nowMinus10mins = DateTime.UtcNow.AddMinutes(-10);
			DateTime nowPlus10mins = DateTime.UtcNow.AddMinutes(10);
			RefreshToken refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);


			// Assert
			string errorMessage = "The refreshToken created is out of +-10mins Range.";
			AssertX.InRange(refreshToken.Created, nowMinus10mins, nowPlus10mins, errorMessage);
		}
		[Theory]
		[InlineData("127.0.0.1")]
		[InlineData("111.97.231.15")]
		[InlineData("196.65.125.9")]
		public void TC0006_GenerateRefreshTokenAndCheckExpiryDateTime(string ipAddress)
		{
			// Arange / Act
			DateTime sevenDaysMinus10mins = DateTime.UtcNow.AddMinutes(-10).AddDays(this._appSettings.RefreshTokenExpiresDuration);
			DateTime sevenDaysPlus10mins = DateTime.UtcNow.AddMinutes(10).AddDays(this._appSettings.RefreshTokenExpiresDuration);
			RefreshToken refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);


			// Assert
			string errorMessage = "The refreshToken expiry is out of +7Days(+-10mins) Range.";
			AssertX.InRange(refreshToken.Expires, sevenDaysMinus10mins, sevenDaysPlus10mins, errorMessage);
		}
	}
}