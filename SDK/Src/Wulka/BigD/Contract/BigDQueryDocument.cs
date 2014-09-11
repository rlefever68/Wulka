using Newtonsoft.Json.Linq;

namespace Wulka.BigD.Contract
{
	/// <summary>
	/// This is used to hold only metadata about a document retrieved from view queries.
	/// </summary>
	public class BigDQueryDocument : BigDDocument
	{
		public string Key { get; set; }

		public override void ReadJson(JObject obj)
		{
//            _id = obj["id"].Value<string>();
			Id = obj["_id"].Value<string>();
			Key = obj["key"].Value<string>();
			var tmp = obj["value"];
			Rev = tmp.ToString() == "null" ? null : tmp.Value<JObject>()["_rev"].Value<string>(); //Rev is null if the value emitted is not doc or does not contain _rev
		}
	}
}