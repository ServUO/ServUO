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

        public List<Mobile> region_mobile;

        public bool m_WaveStatus;

        public bool WaveStatus
        {
            set { m_WaveStatus = value; }
            get { return m_WaveStatus; }
        }

        public BattleRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override void OnDeath(Mobile m)
        {
            base.OnDeath(m);

            bool nomaster = m is BaseCreature && ((BaseCreature)m).GetMaster() == null;

            if (BattleSpawner.Active && nomaster && Spawner != null)
            {
                Timer.DelayCall<BaseCreature>(TimeSpan.FromSeconds(.25), Spawner.RegisterDeath, (BaseCreature)m);
            }

            if (m is BritannianInfantry || m is TribeWarrior || m is TribeShaman || m is MyrmidexDrone || m is MyrmidexWarrior)
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

        public void c(BattleSpawner spawner)
        {
            region_mobile = this.GetMobiles();

            bool see = region_mobile.Any(k => (k.AccessLevel == AccessLevel.Player && (k is PlayerMobile || (k is BaseCreature && ((BaseCreature)k).GetMaster() is PlayerMobile))) && region_mobile.Any(z => (spawner._MyrmidexTypes.Contains(z.GetType()) || spawner._TribeTypes.Contains(z.GetType())) && k.InRange(z.Location, 24)));
            
            if (!see)
            {
                region_mobile.Where(z => spawner._MyrmidexTypes.Contains(z.GetType()) || spawner._TribeTypes.Contains(z.GetType())).ToList().ForEach(x => x.Frozen = true);
                WaveStatus = false;
            }
            else
            {
                region_mobile.Where(z => spawner._MyrmidexTypes.Contains(z.GetType()) || spawner._TribeTypes.Contains(z.GetType())).ToList().ForEach(x => x.Frozen = false);
                WaveStatus = true;
            }
        }
    }
}