using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Regions;

namespace Server.Engines.VoidPool
{
    public class VoidPoolRegion : BaseRegion
	{
		private static Rectangle2D[] Bounds = new Rectangle2D[]
		{
			new Rectangle2D(5383, 1948, 234, 95),
		};
		
		public VoidPoolController Controller { get; private set; }
	
		public VoidPoolRegion(VoidPoolController controller, Map map) : base("Void Pool", map, Region.DefaultPriority, Bounds)
		{
			Controller = controller;
		}
		
		public void SendRegionMessage(int localization)
		{
			List<Mobile> list = GetPlayers();
			list.ForEach(m => m.SendLocalizedMessage(localization));
			list.Clear();
			list.TrimExcess();
		}
		
		public void SendRegionMessage(int localization, string args)
		{
			List<Mobile> list = GetPlayers();
			list.ForEach(m => m.SendLocalizedMessage(localization, args));
			list.Clear();
			list.TrimExcess();
		}

        public void SendRegionMessage(string message)
        {
            List<Mobile> list = GetPlayers();
            list.ForEach(m => m.SendMessage(0x25, message));
            list.Clear();
            list.TrimExcess();
        }
		
		public override void OnDeath(Mobile m)
		{
            if (m is BaseCreature && !((BaseCreature)m).Controlled && !((BaseCreature)m).Summoned && Controller != null && Controller.OnGoing)
            {
                Controller.OnCreatureKilled((BaseCreature)m);

                if (m is CovetousCreature && ((CovetousCreature)m).VoidSpawn)
                {
                    int wave = ((CovetousCreature)m).Level;
                    double bump = wave > 10 ? ((double)Math.Min(60, wave - 10) / 1000.0) : 0;
                    double chance = 0.001 + bump;

                    if (chance > Utility.RandomDouble())
                    {
                        Mobile mob = DemonKnight.FindRandomPlayer((BaseCreature)m);

                        if (mob != null)
                        {
                            Item artifact = VoidPoolRewards.DropRandomArtifact();

                            if (artifact != null)
                            {
                                Container pack = mob.Backpack;

                                if (pack == null || !pack.TryDropItem(mob, artifact, false))
                                    mob.BankBox.DropItem(artifact);

                                mob.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
                            }
                        }
                    }
                }
            }
				
			base.OnDeath(m);
		}
		
		public override bool OnDoubleClick(Mobile m, object o)
		{
			if(o is Corpse && m.AccessLevel == AccessLevel.Player)
			{
				Corpse c = o as Corpse;
				
				if(c.Owner == null || (c.Owner is CovetousCreature && ((CovetousCreature)c.Owner).VoidSpawn))
				{
					c.LabelTo(m, 1152684); // There is no loot on the corpse.
					return false;
				}
			}
			
			return base.OnDoubleClick(m, o);
		}
	
		public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            global = LightCycle.DungeonLevel;
        }

        public override bool CanUseStuckMenu(Mobile m)
        {
            return false;
        }

        public override bool CheckTravel(Mobile m, Point3D newLocation, TravelCheckType travelType)
        {
            if (m.AccessLevel > AccessLevel.Player)
                return true;

            switch (travelType)
            {
                default: return true;
                case TravelCheckType.RecallTo: 
                case TravelCheckType.GateTo:
                case TravelCheckType.Mark: return false;
            }
        }
	}
}