using Server.Mobiles;
using Server.Multis;
using Server.Regions;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.Spellweaving
{
    public class WildfireSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Wildfire", "Haelyn",
            -1,
            false);
        public WildfireSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.5);
        public override double RequiredSkill => 66.0;
        public override int RequiredMana => 50;
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Point3D p)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckSequence())
            {
                int level = GetFocusLevel(Caster);
                double skill = Caster.Skills[CastSkill].Value;

                int tiles = 5 + level;
                int damage = 10 + (int)Math.Max(1, (skill / 24)) + level;
                int duration = (int)Math.Max(1, skill / 24) + level;

                for (int x = p.X - tiles; x <= p.X + tiles; x += tiles)
                {
                    for (int y = p.Y - tiles; y <= p.Y + tiles; y += tiles)
                    {
                        if (p.X == x && p.Y == y)
                            continue;

                        Point3D p3d = new Point3D(x, y, Caster.Map.GetAverageZ(x, y));

                        if (CanFitFire(p3d, Caster))
                            new FireItem(duration).MoveToWorld(p3d, Caster.Map);
                    }
                }

                Effects.PlaySound(p, Caster.Map, 0x5CF);

                NegativeAttributes.OnCombatAction(Caster);

                new InternalTimer(this, Caster, p, damage, tiles, duration).Start();
            }

            FinishSequence();
        }

        private bool CanFitFire(Point3D p, Mobile caster)
        {
            if (!Caster.Map.CanFit(p, 12, true, false))
                return false;
            if (BaseHouse.FindHouseAt(p, caster.Map, 20) != null)
                return false;
            foreach (RegionRect r in caster.Map.GetSector(p).RegionRects)
            {
                if (!r.Contains(p))
                    continue;
                GuardedRegion reg = (GuardedRegion)Region.Find(p, caster.Map).GetRegion(typeof(GuardedRegion));
                if (reg != null && !reg.Disabled)
                    return false;
            }
            return true;
        }

        private static readonly Dictionary<Mobile, long> m_Table = new Dictionary<Mobile, long>();
        public static Dictionary<Mobile, long> Table => m_Table;

        public static void DefragTable()
        {
            List<Mobile> mobiles = new List<Mobile>(m_Table.Keys);

            foreach (Mobile m in mobiles)
            {
                if (Core.TickCount - m_Table[m] >= 0)
                    m_Table.Remove(m);
            }

            ColUtility.Free(mobiles);
        }

        public class InternalTarget : Target
        {
            private readonly WildfireSpell m_Owner;
            public InternalTarget(WildfireSpell owner)
                : base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile m, object o)
            {
                if (o is IPoint3D)
                {
                    m_Owner.Target(new Point3D((IPoint3D)o));
                }
            }

            protected override void OnTargetFinish(Mobile m)
            {
                m_Owner.FinishSequence();
            }
        }

        public class InternalTimer : Timer
        {
            private readonly Spell m_Spell;
            private readonly Mobile m_Owner;
            private readonly Point3D m_Location;
            private readonly int m_Damage;
            private readonly int m_Range;
            private int m_LifeSpan;
            private readonly Map m_Map;

            public InternalTimer(Spell spell, Mobile owner, Point3D location, int damage, int range, int duration)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), duration)
            {
                m_Spell = spell;
                m_Owner = owner;
                m_Location = location;
                m_Damage = damage;
                m_Range = range;
                m_LifeSpan = duration;
                m_Map = owner.Map;
            }

            protected override void OnTick()
            {
                if (m_Owner == null || m_Map == null || m_Map == Map.Internal)
                    return;

                m_LifeSpan -= 1;
                List<Mobile> targets = GetTargets().Where(m => BaseHouse.FindHouseAt(m.Location, m.Map, 20) == null).ToList();
                int count = targets.Count;

                foreach (Mobile m in targets)
                {
                    m_Owner.DoHarmful(m);

                    if (m_Map.CanFit(m.Location, 12, true, false))
                        new FireItem(m_LifeSpan).MoveToWorld(m.Location, m_Map);

                    Effects.PlaySound(m.Location, m_Map, 0x5CF);
                    double sdiBonus = (double)AosAttributes.GetValue(m_Owner, AosAttribute.SpellDamage) / 100;

                    if (m is PlayerMobile && sdiBonus > .15)
                        sdiBonus = .15;

                    int damage = m_Damage + (int)(m_Damage * sdiBonus);

                    if (count > 1)
                        damage /= Math.Min(3, count);

                    AOS.Damage(m, m_Owner, damage, 0, 100, 0, 0, 0, 0, 0, DamageType.SpellAOE);
                    Table[m] = Core.TickCount + 1000;
                }

                ColUtility.Free(targets);
            }

            private IEnumerable<Mobile> GetTargets()
            {
                DefragTable();

                return m_Spell.AcquireIndirectTargets(m_Location, m_Range).OfType<Mobile>().Where(m => !m_Table.ContainsKey(m));
            }
        }

        public class FireItem : Item
        {
            public FireItem(int duration)
                : base(Utility.RandomBool() ? 0x398C : 0x3996)
            {
                Movable = false;
                Timer.DelayCall(TimeSpan.FromSeconds(duration), Delete);
            }

            public FireItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                Delete();
            }
        }
    }
}
