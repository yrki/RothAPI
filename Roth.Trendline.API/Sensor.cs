using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roth.Touchline.API
{
	public class Sensor
	{
		public string Name { get; set; }
		public double CurrentTemperature { get; set; }
		public string CurrentProgram { get; set; }
		public string TemperatureUnits { get; set; }
		public int Id { get; internal set; }
	}
}
