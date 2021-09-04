using System.Collections.Generic;
using System.Threading.Tasks;

using Backend.Models.UserManagement;
using Backend.Models;


namespace Backend.Services
{
	public interface IUserService
	{
		// Authentication

		/// <summary>
		/// Authenticates the user with Username and password. It also generates the first accessToken [JWT] and [RefreshToken]
		/// </summary>
		/// <param name="model">[AuthenticateRequest] Input JSON like object with Username and Password.</param>
		/// <param name="ipAddress">[string] IpAddress who requests the action. - It is used for the [RefreshToken]</param>
		/// <returns>Returns a Task - [AuthenticationResponse] object, containing some: User data, accessToken [JWT] and the newly activated [RefreshToken]</returns>
		Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
		/// <summary>
		/// Checks whether the refreshToken fed to it is active or not. If active it returns a new accessToken [JWT] and issues a new [RefreshToken].
		/// <para>It also removes the old / outdated / non-active / revoked [RefreshToken] RefreshTokens from the MongoDB databse.</para>
		/// </summary>
		/// <param name="refreshTokenStringRepresentation">[string] String representation of the queryable [RefreshToken]</param>
		/// <param name="ipAddress">[string] IpAddress who requests the action.</param>
		/// <returns>Returns a Task - [AuthenticationResponse] object, containing some: User data, accessToken [JWT] and the newly activated [RefreshToken]</returns>
		Task<AuthenticateResponse> RefreshToken(string refreshTokenStringRepresentation, string ipAddress);
		/// <summary>
		/// Revokes a refreshToken. You give it a [string], the [refreshToken.Token] field equivalent and it runs a query against the MongoDB database.
		/// <para>The IpAddress input string serves to mark who was the one revoking the token.</para>
		/// </summary>
		/// <param name="token">[string] String representation of the token to be removed.</param>
		/// <param name="ipAddress">[string] IpAddress who revokes the token.</param>
		/// <returns>Returns a Task, equivalent to a void, but awaitable.</returns>
		Task RevokeToken(string token, string ipAddress);

		// User methods

		/// <summary>
		/// Gets all the users from the MongoDB database. Asynchronously.
		/// </summary>
		/// <returns>Returns a task with [List &lt; UserEntity &gt; ]</returns>
		Task<List<UserEntity>> GetAllUsersAsync();
		/// <summary>
		/// Gets a single user from the MongoDB database by ID. Asynchronously.
		/// </summary>
		/// <param name="id">[string] User ID - MongoDB ObjectId</param>
		/// <returns>Returns a task with [UserEntity]</returns>
		Task<UserEntity> GetUserByIdAsync(string id);
		/// <summary>
		/// Registers a user to the MongoDB database. Asynchronously.
		/// </summary>
		/// <param name="model">[RegisterRequest] model - Input for the registration script.</param>
		/// <returns>Returns a Task, equivalent to a void, but awaitable.</returns>
		Task Register(RegisterRequest model);
	}
}