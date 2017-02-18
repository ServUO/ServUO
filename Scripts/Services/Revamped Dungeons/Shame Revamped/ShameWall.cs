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

        public static void AddTeleporters(ShameWall wall)
        {
            Map map = wall.Map;

            if (map == null || map == Map.Internal)
                return;

            foreach (var component in wall.Components)
            {
                foreach (Point3D[] pnts in _TeleportLocs)
                {
                    if (component.Location == pnts[0])
                    {
                        var teleporter = new ConditionTeleporter();
                        teleporter.DeadOnly = true;
                        teleporter.MapDest = map;
                        teleporter.PointDest = pnts[2];
                        teleporter.DisableMessage = true;
                        teleporter.MoveToWorld(pnts[1], map);
                        WeakEntityCollection.Add("newshame", teleporter);
                    }
                }
            }
        }

        private static Point3D[][] _TeleportLocs =
        {
            new Point3D[] { new Point3D(5402, 82, 10), new Point3D(5402, 81, 10), new Point3D(5402, 83, 10) },
            new Point3D[] { new Point3D(5403, 82, 10), new Point3D(5403, 81, 10), new Point3D(5403, 83, 10) },
            new Point3D[] { new Point3D(5404, 82, 10), new Point3D(5404, 81, 10), new Point3D(5404, 83, 10) },
            new Point3D[] { new Point3D(5405, 82, 10), new Point3D(5405, 81, 10), new Point3D(5405, 83, 10) },

            new Point3D[] { new Point3D(5465, 25, -10), new Point3D(5464, 25, -10), new Point3D(5466, 25, -10) },
            new Point3D[] { new Point3D(5465, 26, -10), new Point3D(5464, 26, -10), new Point3D(5466, 26, -10) },
            new Point3D[] { new Point3D(5465, 27, -10), new Point3D(5464, 27, -10), new Point3D(5466, 27, -10) },
            new Point3D[] { new Point3D(5465, 28, -10), new Point3D(5464, 28, -10), new Point3D(5466, 28, -10) },

            new Point3D[] { new Point3D(5618, 57, 0), new Point3D(5618, 58, 0), new Point3D(5618, 56, 0) },
            new Point3D[] { new Point3D(5619, 57, 0), new Point3D(5619, 58, 0), new Point3D(5619, 56, 0) },
            new Point3D[] { new Point3D(5620, 57, 0), new Point3D(5620, 58, 0), new Point3D(5620, 56, 0) },
        };
		
		public ShameWall(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)1);
			
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

            if (version == 0)
                Timer.DelayCall(TimeSpan.FromSeconds(20), () => AddTeleporters(this));
		}
	}
}