using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Backend.Models
{
	public class NaturalDNASequenceEntity
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("sequenceName")]
		[JsonProperty("sequenceName")]
		public string SequenceName { get; set; }
		[BsonElement("sequence")]
		[JsonProperty("sequence")]
		public string Sequence { get; set; }
	}
}