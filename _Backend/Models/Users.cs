using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Backend.Models
{
	public class UserModelEntity
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("userName")]
		[JsonProperty("userName")]
		public string userName { get; set; }
		[BsonElement("passwordHashed")]
		[JsonProperty("passwordHashed")]
		public string passwordHashed { get; set; }
	}
}