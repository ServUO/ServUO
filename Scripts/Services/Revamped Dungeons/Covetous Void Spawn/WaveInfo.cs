using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.VoidPool
{
	public class WaveInfo
	{
		public int Wave { get; set; }
		public bool Cleared { get; set; }
		public List<BaseCreature> Creatures { get; private set; }
		public List<Mobile> Credit { get; set; }
	
		public WaveInfo(int index, List<BaseCreature> list)
		{
			Wave = index;
			Creatures = list;
			
			Credit = new List<Mobile>();
		}
	}
}