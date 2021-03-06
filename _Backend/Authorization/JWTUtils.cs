using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

using Backend.Models;
using Backend.Models.Authentication;
using Backend.Helpers;

namespace Backend.Authorization
{
	public interface IJWTUtils
	{
		/// <summary>
		/// This function generates an accessToken, which takes the <c>UserEntity</c> input and puts the <c>user.Id</c> as claim into the token.
		/// </summary>
		/// <param name="user">UserEntity type. The user that user.Id we want in the token as claim.</param>
		/// <returns>([string], [string]) String type accessToken (JWT token) || String tokenID</returns>
		public (string, string) GenerateAccessToken(UserEntity user);
		/// <summary>
		/// Validates the accessToken (JWTToken) fed as string. Validates the lifecycle, signing key.
		/// <para>Also retrieves the user.Id from the claims.</para>
		/// </summary>
		/// <param name="token">JWT token as [string]</param>
		/// <returns>([string], [string]) user.Id as [string] or null ||  tokenId as [string] or null </returns>
		public (string, string) ValidateAccessToken(string token);
		/// <summary>
		/// Generates a [RefreshToken] refreshToken and returns it as the class object.
		/// </summary>
		/// <param name="ipAddress">[string] IpAddress that asked to generate the token at hand.</param>
		/// <param name="issuedJWT">[string] The issued JWT token that is the pair of the refreshToken being generated.</param>
		/// <returns>[RefreshToken]</returns>
		public RefreshToken GenerateRefreshToken(string ipAddress, string issuedJWTTokenId);

	}

	public class JWTUtils : IJWTUtils
	{
		private readonly AppSettings _appSettings;

		public JWTUtils(IOptions<AppSettings> appSettings)
		{
			this._appSettings = appSettings.Value;
		}
		public (string, string) GenerateAccessToken(UserEntity user)
		{
			JwtSecurityTokenHandler tokenHandler = new();
			byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);

			using RNGCryptoServiceProvider rngCryptoServiceProvider = new();
			byte[] randomBytes = new byte[64];
			rngCryptoServiceProvider.GetBytes(randomBytes);
			string tokenID = Convert.ToBase64String(randomBytes);

			SecurityTokenDescriptor tokenDescriptor = new()
			{
				Subject = new ClaimsIdentity(new[] {    new Claim("userID", user.Id.ToString()),
														new Claim("tokenID", tokenID)}),
				Expires = DateTime.UtcNow.AddMinutes(15),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
			};
			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

			return (tokenHandler.WriteToken(token), tokenID);
		}
		public (string, string) ValidateAccessToken(string token)
		{
			// Returning a null if the token does not exist.
			if (token == null) return (null, null);

			// Handling the token
			JwtSecurityTokenHandler tokenHandler = new();
			byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);

			// Validating the token
			// BUG: Maybe this can be rewritten without try-catch
			// FIXME: Check whether we need to access the user or troubleshoot it, or hash is just magic?
			try
			{
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					// Setting the "ClockSkew" to 0 will ensure, that tokens expiry exactly at expiration time, not delayed
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				JwtSecurityToken JWTToken = (JwtSecurityToken)validatedToken;
				string userId = JWTToken.Claims.First(_claim => _claim.Type == "userID").Value;
				string tokenId = JWTToken.Claims.First(_claim => _claim.Type == "tokenID").Value;

				// Returning the userID if validation was successful
				return (userId, tokenId);
			}
			catch
			{
				// Return null if validation fails.
				return (null, null);
			}
		}

		public RefreshToken GenerateRefreshToken(string ipAddress, string issuedJWTTokenId)
		{
			// Generate token that is valid for 7 days
			using RNGCryptoServiceProvider rngCryptoServiceProvider = new();

			byte[] randomBytes = new byte[64];
			rngCryptoServiceProvider.GetBytes(randomBytes);
			RefreshToken refreshToken = new()
			{
				Token = Convert.ToBase64String(randomBytes),
				IssuedJWTTokenId = issuedJWTTokenId,
				Expires = DateTime.UtcNow.AddDays(this._appSettings.RefreshTokenExpiresDuration),
				Created = DateTime.UtcNow,
				CreatedByIp = ipAddress
			};

			return refreshToken;
		}
	}
}