using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Roth.Touchline.API
{
	public class Commands
	{
		// http://dev.n0ll.com/2014/10/roth-touchline-time-to-get-owned/

		// Search for all systems
		// Get number of units
		// Get current temperature
		// 

		// What can be done:
		// - Pull temperature from sensors every x second
		// - Schedule times and set temperature schedule
		//			- Daily schedule
		//			- Weekly schedule
		//			- Connect to weekends / weekdays when setting calendar
		//			- Set weekends 
		//			- Override system
		// - Calculate delay in start setting temperature until teperature is reached
		// - Connect to Yr (Set temperatures based on outside temperature)
		// - Log temperature data - every 5 minutes (?)
		// - Log YR-temperature
		// - Preset schedules
		// - Estimate heating time based on outside temperature
		// - 
		// Charts: 
		// https://google-developers.appspot.com/chart/interactive/docs/gallery/gauge

		public string Ip { get; set; }
		public int NumberOfControllers { get; set; }

		public List<Sensor> GetSensorInformation()
		{

			var names = GetSensorInformation("G{0}.Name");
			var temperatures = GetSensorInformation("G{0}.RaumTemp");
			var unitsOfMeasure = GetSensorInformation("G{0}.TempSIUnit");
			var programs = GetSensorInformation("G{0}.WeekProg");

			List<Sensor> sensors = new List<Sensor>();

			for (int i = 0; i < NumberOfControllers; i++)
			{
				var sensor = new Sensor();
				sensor.Id = i;
				sensor.Name = names.ItemList[i].Value;
				sensor.CurrentTemperature = Convert.ToDouble(temperatures.ItemList[i].Value) / 100;
				sensor.TemperatureUnits = unitsOfMeasure.ItemList[i].Value;
				sensor.CurrentProgram = programs.ItemList[i].Value;

				sensors.Add(sensor);
			}

			return sensors;
		}

		private int GetNumberOfControllers()
		{
			var model = new Model();
			model.ItemList = new List<ItemList>();

			var itemList = new ItemList();
			itemList.Name = "totalNumberOfDevices";
			model.ItemList.Add(itemList);

			var response = ReadValues(model);

			return Convert.ToInt32(response.ItemList.FirstOrDefault().Value);
		}

		private Model GetSensorInformation(string command)
		{
			if (NumberOfControllers == 0)
			{
				NumberOfControllers = GetNumberOfControllers();
			}

			var request = new Model();
			request.ItemList = new List<ItemList>();
			for (int i = 0; i < NumberOfControllers; i++)
			{
				var item = new ItemList()
				{
					Name = string.Format(command, i)
				};

				request.ItemList.Add(item);
			}

			var response = ReadValues(request);


			return response;
		}

		public void SetTemperature(int sensorId, double temperature)
		{
			//TODO: Validate input parameters

			string command = string.Format("G{0}.SollTemp", sensorId);
			WriteValues(command, (temperature * 100).ToString());
		}

		public void SetProgram(int sensorId, WeekProgram program)
		{
			string command = string.Format("G{0}.WeekProg", sensorId);
			WriteValues(command, program.GetTypeCode().ToString());
		}

		public void SetMode(int sensorId, Mode mode)
		{
			string command = string.Format("G{0}.OPMode", sensorId);
			WriteValues(command, mode.GetTypeCode().ToString());
		}

		public enum WeekProgram
		{
			Off = 0,
			ProI = 1,
			ProII = 2,
			ProIII = 3
		}

		public enum Mode
		{
			Day = 0,
            Night = 1,
			OffHoliday = 2
		}


		private  Model ReadValues(Model requestModel)
		{
			string url = "http://" + Ip + "/cgi-bin/ILRReadValues.cgi";
			var request = Serialize(requestModel);
			Model model;

			using (var client = new System.Net.WebClient())
			{
				client.Headers.Add("Content-Type", "text/xml");
				client.Headers.Add("User-Agent", "SpiderControl/1.0(iniNet-Solutions GmbH)");
				var response = client.UploadData(url, "POST", request);
				model = Deserialize(response);
			}

			return model;
		}

		private void WriteValues(string command, string value)
		{
			string url = string.Format("http://{0}/cgi-bin/writeVal.cgi?{1}={2}");

			using (var client = new System.Net.WebClient())
			{
				client.Headers.Add("User-Agent", "Mozilla/4.0 (Windows8 6.2)Java/1.6.0_43");
				client.DownloadData(url);
			}
		}

		private byte[] Serialize(Model model)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Model));
			MemoryStream stream = new MemoryStream();
			serializer.Serialize(stream, model);
			
			return stream.GetBuffer();

		}

		private Model Deserialize(byte[] bytes)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Model));
			MemoryStream stream = new MemoryStream(bytes);
			
			var model = (Model)serializer.Deserialize(stream);

			return model;
		}

	}
}
