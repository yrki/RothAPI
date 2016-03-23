using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Roth.Touchline.API
{
	public class ScanNetwork
	{
		public string SearchForServer()
		{
			//Dns.
			var o = Dns.GetHostEntry("THOMASPC");
			//Dns.GetHostByAddress("");

			// Gets the IP-address.
			// o.AddressList.FirstOrDefault(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString()

			return o.HostName;
		}
	}
}
