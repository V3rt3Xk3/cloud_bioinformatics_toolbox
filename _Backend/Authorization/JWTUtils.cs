using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Backend.Models;
using Backend.Helpers;

namespace Backend.Authorization
{
	public interface IJWTUtils
	{
		public string GenerateToken(UserEntity user);
		public string ValidateToken(string token);
	}

	public class JWTUtils : IJWTUtils
	{
		private readonly AppSettings _appSettings;

		public JWTUtils(IOptions<AppSettings> appSettings)
		{
			this._appSettings = appSettings.Value;
		}

		public string GenerateToken(UserEntity user)
		{
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);
			SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
				Expires = DateTime.UtcNow.AddDays(7),
				// BUG: I would like this to go Assymetric at somepoint
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
			};
			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		public string ValidateToken(string token)
		{
			// Returning a null if the token does not exist.
			if (token == null) return null;

			// Handling the token
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);

			// Validating the token
			// BUG: Maybe this can be rewritten without try-catch
			// FIXME: Check whether we need to access the user or troubleshoot it, or hash is just magic?
			try
			{
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					// BUG: Then again, I want it to be assymetric.
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					// Setting the "ClockSkew" to 0 will ensure, that tokens expiry exactly at expiration time, not delayed
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				JwtSecurityToken JWTToken = (JwtSecurityToken)validatedToken;
				string userId = JWTToken.Claims.First(hiddenId => hiddenId.Type == "Id").Value;

				// Returning the userID if validation was successful
				return userId;
			}
			catch
			{
				// Return null if validation fails.
				return null;
			}
		}
	}
}