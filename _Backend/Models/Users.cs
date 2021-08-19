using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Backend.Models
{
	public class UserEntity
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("userName")]
		[JsonProperty("userName")]
		public string Username { get; set; }


		[JsonIgnore]
		public string PasswordHashed { get; set; }
	}
}