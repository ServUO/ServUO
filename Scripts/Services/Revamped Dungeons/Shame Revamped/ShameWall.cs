using System;
using Server;
using Server.Mobiles;
using Server.Engines.Points;
using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.ShameRevamped
{
	public class ShameWall : BaseAddon
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public CaveTroll Troll { get; set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D StartSpot { get; set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Map StartMap { get; set; }
			
		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D TrollSpawnLoc { get; set; }
		
		public ShameWall(Dictionary<Point3D, int> details, Point3D startSpot, Point3D trollSpawnLoc, Map map)
		{
			foreach(KeyValuePair<Point3D, int> kvp in details)
			{
				AddComponent(new AddonComponent(kvp.Value), kvp.Key.X, kvp.Key.Y, kvp.Key.Z);
			}
			
			StartSpot = startSpot;
			StartMap = map;
			TrollSpawnLoc = trollSpawnLoc;
			
			SpawnTroll();
		}
		
		public void OnTrollKilled()
		{
			this.Z -= 50;
			this.Visible = false;
		
			Timer.DelayCall(TimeSpan.FromMinutes(2), Reset);
		
			Troll = null;
		}
		
		public void Reset()
		{
			MoveToWorld(StartSpot, StartMap);
			Visible = true;
			SpawnTroll();
		}
		
		private void SpawnTroll()
		{
			if(Troll == null || Troll.Deleted || !Troll.Alive)
			{
				Troll = new CaveTroll(this);
				Troll.MoveToWorld(TrollSpawnLoc, StartMap);

                Troll.Home = TrollSpawnLoc;
                Troll.RangeHome = 8;
			}
		}

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            base.OnComponentUsed(component, from);

            if (Troll == null || Troll.Deleted || !Troll.Alive)
                Reset();
        }
		
		public ShameWall(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			writer.Write(Troll);
			writer.Write(StartSpot);
			writer.Write(StartMap);
			writer.Write(TrollSpawnLoc);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			Troll = reader.ReadMobile() as CaveTroll;
			StartSpot = reader.ReadPoint3D();
			StartMap = reader.ReadMap();
			TrollSpawnLoc = reader.ReadPoint3D();

            if (Troll != null)
                Troll.Wall = this;

            if (this.Location != StartSpot || Troll == null || Troll.Deleted || !Troll.Alive)
				Reset();
		}
	}
}