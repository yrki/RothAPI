using System.Collections.Generic;
using System.Xml.Serialization;

namespace Roth.Touchline.API
{
	[XmlRoot("body")]
	public class Model
	{
		[XmlArray("item_list")]
		[XmlArrayItem("i")]
		public List<ItemList> ItemList { get; set; }
	}


	public class ItemList
	{
		[XmlElement("n")]
		public string Name { get; set; }

		[XmlElement("v")]
		public string Value { get; set; }
	}
}
