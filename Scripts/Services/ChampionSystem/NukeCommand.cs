using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Commands;
using Server.Items;

namespace Server.Services.ChampionSystem
{
	// This is a funciton for testing the champ spawn system
	public class NukeCommand
	{
		private static void Initialize()
		{
			CommandSystem.Register("Nuke", AccessLevel.GameMaster, new CommandEventHandler(Nuke_OnCommand));
		}

		private static void Nuke_OnCommand(CommandEventArgs e)
		{
			foreach (Mobile mob in e.Mobile.Map.GetMobilesInRange(e.Mobile.Location, 18))
			{
				Effects.SendLocationParticles(EffectItem.Create(mob.Location, mob.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
				Effects.PlaySound(mob, mob.Map, 0x208);
				mob.Damage(60000, e.Mobile);
			}
		}
	}
}