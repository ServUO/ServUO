using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using Server.Regions;
using System.Xml;
using System.Linq;

namespace Server.Engines.MyrmidexInvasion
{
	public class BattleRegion : StygianAbyssRegion
	{
		public BattleSpawner Spawner { get; set; }

		public bool WaveStatus { get; set; }

		public BattleRegion(XmlElement xml, Map map, Region parent)
			: base(xml, map, parent)
		{ }

		public override void OnDeath(Mobile m)
		{
			base.OnDeath(m);

			Mobile master = m is BaseCreature ? ((BaseCreature)m).GetMaster() : null;

			if (BattleSpawner.Active && master == null && Spawner != null)
			{
				Timer.DelayCall(Spawner.RegisterDeath, (BaseCreature)m);
			}

			if (m.Corpse != null && !m.Corpse.Deleted && (BattleSpawner._MyrmidexTypes.Contains(m.GetType()) || BattleSpawner._TribeTypes.Contains(m.GetType())))
			{
				Mobile killer = m.LastKiller;

				if (killer is BaseCreature && !(((BaseCreature)killer).GetMaster() is PlayerMobile))
				{
					m.Corpse.Delete();
				}
			}
		}

		public override void OnExit(Mobile m)
		{
			if (m is PlayerMobile && Spawner != null)
				Spawner.OnLeaveRegion((PlayerMobile)m);

			base.OnExit(m);
		}

		public override bool OnDamage(Mobile m, ref int Damage)
		{
			Mobile attacker = m.FindMostRecentDamager(false);

			if (MyrmidexInvasionSystem.IsEnemies(m, attacker) && EodonianPotion.IsUnderEffects(attacker, PotionEffect.Kurak))
			{
				Damage *= 3;

				if (Damage > 0)
					m.FixedEffect(0x37B9, 10, 5);
			}

			return base.OnDamage(m, ref Damage);
		}

		public void ValidateSpawn()
		{
			var visible = 0;
		
			var mobiles = GetMobiles().ToList();
			
			mobiles.RemoveAll(m => m.AccessLevel >= AccessLevel.Counselor);
			
			foreach(var m in mobiles.Where(m => !m.Player && (BattleSpawner._MyrmidexTypes.Contains(m.GetType()) || BattleSpawner._TribeTypes.Contains(m.GetType()))))
			{
				if(!mobiles.Any(o => o.Player && o.CanSee(m) && o.InRange(m, Core.GlobalMaxUpdateRange)))
				{
					m.Frozen = true;
				}
				else
				{
					m.Frozen = false;
					
					++visible;
				}
			}
			
			WaveStatus = visible > 0;
			
			mobiles.Clear();
			mobiles.TrimExcess();
		}
    }
}
