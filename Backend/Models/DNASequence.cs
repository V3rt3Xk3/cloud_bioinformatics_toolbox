using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models
{
	public class NaturalDNASequence
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("SequenceName")]
		public string SequenceName { get; set; }
		public string Sequence { get; set; }
	}
}