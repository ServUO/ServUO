using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Services.ChampionSystem
{
	// This is a funciton for testing the champ spawn system
	public class NukeCommand
	{
		public static void Initialize()
		{
			CommandSystem.Register("Nuke", AccessLevel.GameMaster, new CommandEventHandler(Nuke_OnCommand));
		}

		private static void Nuke_OnCommand(CommandEventArgs e)
		{
			foreach (Mobile mob in e.Mobile.Map.GetMobilesInRange(e.Mobile.Location, 18))
			{
				if (mob is PlayerMobile)
					continue;
				Effects.SendLocationParticles(EffectItem.Create(mob.Location, mob.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
				Effects.PlaySound(mob, mob.Map, 0x208);
				mob.Damage(60000, e.Mobile);
			}
			new InternalTimer(e.Mobile.Location, e.Mobile.Map).Start();
		}

		private class InternalTimer : Timer
		{
			private Point3D m_Location;
			private Map m_Map;

			public InternalTimer(Point3D p, Map m)
				: base(TimeSpan.FromSeconds(0.25d))
			{
				m_Location = p;
				m_Map = m;
			}
			protected override void OnTick()
			{
				foreach(Item item in m_Map.GetItemsInRange(m_Location, 18))
				{
					if(item is Corpse)
					{
						item.Delete();
					}
				}
				Stop();
			}
		}
	}
}