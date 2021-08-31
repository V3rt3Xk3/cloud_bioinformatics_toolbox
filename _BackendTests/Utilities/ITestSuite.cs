using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace BackendTests.Utilities
{
	public interface ITestSuite
	{

		Task TestSuiteSetUp();

		/// <summary>
		/// Cleans up the MongoDB "cloud_bioinformatics_test" test database. | Drops it.
		/// </summary>
		public static void TestCase_DatabaseCleanUp()
		{
			ITestSuite.MongoDBCleanUp();
		}
		// NOTE: As it turns out, Interfaces can have their own static implementations as well.
		/// <summary>
		/// Cleans up the MongoDB "cloud_bioinformatics_test" test database. | Drops it.
		/// </summary>
		public static void MongoDBCleanUp()
		{
			string connectionString = "mongodb://cloud_bioinformaitcs_mongo_dev:%2333FalleN666%23@localhost:27017/?authSource=cloud_bioinformatics_test";
			MongoClient client = new(connectionString);

			string DatabaseNamespace = "cloud_bioinformatics_test";
			client.DropDatabase(DatabaseNamespace);
		}
	}
}