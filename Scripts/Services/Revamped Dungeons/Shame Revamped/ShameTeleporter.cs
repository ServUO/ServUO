using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.PartySystem;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.ShameRevamped
{
	public class ShameTeleporter : Teleporter
	{
		public List<Mobile> AccessList { get; set; }
	
		[CommandProperty(AccessLevel.GameMaster)]
		public ShameAltar Altar { get; set; }
		
		public ShameTeleporter(ShameAltar altar, Point3D dest, Map map) : base(dest, map, true)
		{
			Altar = altar;

            AccessList = new List<Mobile>();
		}
		
		public void AddToAccessList(Mobile from)
		{
			if(!AccessList.Contains(from))
				AccessList.Add(from);
		
			if(ShameAltar.AllowParties)
			{
				Party p = Party.Get(from);
				
				if(p != null)
				{
                    foreach (PartyMemberInfo info in p.Members.Where(info => !AccessList.Contains(info.Mobile)))
                    {
                        AccessList.Add(info.Mobile);
                    }
				}
			}
		
			//Timer.DelayCall(TimeSpan.FromMinutes(ShameAltar.CoolDown), ClearAccessList);
		}
		
		private void ClearAccessList()
		{
			if(AccessList != null)
			{
				AccessList.Clear();
				AccessList.TrimExcess();
				AccessList = null;
			}
		}
		
		public override bool OnMoveOver(Mobile m)
		{
			if(Altar == null || m.AccessLevel > AccessLevel.GameMaster)
				return base.OnMoveOver(m);
				
			if(AccessList != null && AccessList.Contains(m))
				return base.OnMoveOver(m);
				
			return true;
		}
		
		public ShameTeleporter(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			writer.Write(AccessList.Count);
			AccessList.ForEach(m => writer.Write(m));
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

            AccessList = new List<Mobile>();
			int count = reader.ReadInt();
			
			if(count > 0)
			{
				AccessList = new List<Mobile>();
				//Timer.DelayCall(TimeSpan.FromMinutes(ShameAltar.CoolDown), ClearAccessList);
				
				for(int i = 0; i < count; i++)
				{
					Mobile m = reader.ReadMobile();
					
					if(m != null)
						AccessList.Add(m);
				}
			}
		}
	}
}