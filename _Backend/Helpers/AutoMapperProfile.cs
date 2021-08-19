using AutoMapper;

using Backend.Models;
using Backend.Models.UserManagement;

namespace Backend.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			// UserEntity -> AuthenticateResponse
			CreateMap<UserEntity, AuthenticateResponse>();

			// RegistrationRequest -> UserEntity
			CreateMap<RegisterRequest, UserEntity>();

			// UpdateRequest -> UserEntity
			CreateMap<UpdateRequest, UserEntity>()
				.ForAllMembers(inputElement => inputElement.Condition(
					(source, destination, property) =>
					{
						// Ignore nulls and empty - string - properties.
						if (property == null) return false;
						if (property.GetType() == typeof(string) && string.IsNullOrEmpty((string)property)) return false;

						return true;
					}
				));
		}
	}
}