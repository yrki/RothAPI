using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Roth.Touchline.API;

namespace Roth.Touchline.TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{

			//var scanNetork = new API.ScanNetwork();
//scanNetork.SearchForServer();

			var commands = new API.Commands();
			commands.Ip = "192.168.1.121";

			var externalTemperature = new ExternalTemperature();
			


			while (true)
			{
				var sensors = commands.GetSensorInformation();

				var temps = new double[sensors.Count];

				var temperature = externalTemperature.GetCurrentOutDoorTemperature(
								latitude: 59.6797139,
								longitude: 10.5255243,
								altitude: 10);


				int counter = 0;
				foreach (var sensor in sensors)
				{
					Console.WriteLine("Sensor: {0}, Temperature = {1} {2}, Current Program = {3}", sensor.Id, sensor.CurrentTemperature,
						sensor.TemperatureUnits, sensor.CurrentProgram);
					temps[counter] = sensor.CurrentTemperature;

					counter++;
				}

				string str = string.Format("{0},{1},{2},{3},{4}", DateTime.Now, temps[0], temps[1], temps[2], temperature);

				using (var file = System.IO.File.AppendText(@"f:\temp\temperaturer.txt"))
				{
					file.WriteLine(str);
				}

				Thread.Sleep(60000);
			}

			Console.ReadKey();
		}



		// Temperaturøkning

		// Utetemperatur = 10;
		// Snitt oppvarmingstid 1 grad v/utetemperatur ca 10 grader.
		// Rom: Nåværende temperatur inne, nåværende utetemperatur, Ønsket oppvarmet til X grader, Ønsket oppvarmet tid = Resultat når skal man skru på.
		// 

		// Data fra Yr
		// Dokumentasjon: http://api.yr.no/weatherapi/locationforecast/1.9/documentation
		// http://api.yr.no/weatherapi/locationforecast/1.9/?lat=59.6797139;lon=10.5255243;msl=10


	}
}
