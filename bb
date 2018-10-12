using System;

using Server;
using Server.Multis;

namespace Server.Items
{
	public class IDOCBoard
	{
		public IDOCBoard()
			: base(0x1234)
		{
		}
		
		public override void OnDoubleClick(Mobile m)
		{
			if(m.InRange(Location, 3))
			{
				var count = GetIDOCCount();
				
				PrivateOverheadMessage(MessageType.Regular, getyellowhue, false, 
					String.Format("There are {0} houses in danger of collapsing!", count), m.NetState); 
			}
		}
		
		private int GetHouses()
		{
			return BaseHouse.GetHouses().Where(h => 
				h.DecayLevel >= DecayLevel.IDOC && 
				h.Map != null && 
				h.Map != Map.Internal &&
				!h.Region.IsPartOf<Regions.GreenAcres>().Count();
		}
		
		public IDOCBoard(Serial serial) 
			: base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			/*int version =*/ reader.ReadInt();
		}
	}
}
