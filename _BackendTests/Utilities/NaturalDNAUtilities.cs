using System.IO;
using Newtonsoft.Json.Linq;


namespace BackendTests.Utilities
{
	public static class NaturalDNA_TestUtilities
	{
		public static string LoadTestData_SingleEntity(int index = 0)
		{

			string dataFilePath = "./Utilities/DataFiles/NaturalDNATestData.json";
			string singleSequenceDocument;

			using (StreamReader readerAgent = new(dataFilePath))
			{
				string jsonInput = readerAgent.ReadToEnd();
				JArray jsonArrayParsed = JArray.Parse(jsonInput);
				singleSequenceDocument = jsonArrayParsed[index].ToString();
			}
			return singleSequenceDocument;
		}
	}
}