using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Mobiles
{
	public class EscortDestinationInfo
	{
		public EscortDestinationInfo(string name, Region region)
		{
			Name = name;
			Region = region;
		}

		public string Name { get; set; }
		public Region Region { get; set; }

		public static EscortDestinationInfo Find(string name)
		{
			if (String.IsNullOrEmpty(name))
				return null;
			Region reg = null;
			foreach (var r in Region.Regions.Where(r => r.Name == name))
				reg = r;

			return new EscortDestinationInfo(name,reg);
		}

		public bool Contains(Point3D location)
		{
			return Region.Regions.Where(r => r.Name == Name).Any(r => r.Contains(location));
		}
	}
}
