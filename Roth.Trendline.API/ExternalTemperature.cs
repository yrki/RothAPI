using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Roth.Touchline.API.YrModels;

namespace Roth.Touchline.API
{
	public class ExternalTemperature
	{
		public double GetCurrentOutDoorTemperature (double longitude, double latitude, int altitude)
		{
			var url = string.Format(@"http://api.yr.no/weatherapi/locationforecast/1.9/?lat={0};lon={1};msl={2}", 
				latitude,
				longitude, 
				altitude);

			weatherdata weather;

			using (var client = new System.Net.WebClient())
			{
				var response = client.DownloadData(url);
				weather = Deserialize(response);
			}

			var temperature = weather.product.FirstOrDefault().time.FirstOrDefault().location.FirstOrDefault().temperature.FirstOrDefault().value;

			return (double)temperature;
		}


		private byte[] Serialize(Model model)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(weatherdata));
			MemoryStream stream = new MemoryStream();
			serializer.Serialize(stream, model);

			return stream.GetBuffer();

		}

		private weatherdata Deserialize(byte[] bytes)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(weatherdata));
			MemoryStream stream = new MemoryStream(bytes);

			var weatherdata = (weatherdata)serializer.Deserialize(stream);

			return weatherdata;
		}
	}
}
