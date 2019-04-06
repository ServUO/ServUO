using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a blade spirit corpse")]
    public class BladeSpirits : BaseCreature
    {
        [Constructable]
        public BladeSpirits()
            : this(false)
        {
        }

        [Constructable]
        public BladeSpirits(bool summoned)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.3, 0.6)
        {
            this.Name = "a blade spirit";
            this.Body = 574;

            bool weak = summoned && Siege.SiegeShard;

            this.SetStr(weak ? 100 : 150);
            this.SetDex(weak ? 100 : 150);
            this.SetInt(100);

            this.SetHits((Core.SE && !weak) ? 160 : 80);
            this.SetStam(250);
            this.SetMana(0);

            this.SetDamage(10, 14);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 70.0);
            this.SetSkill(SkillName.Tactics, 90.0);
            this.SetSkill(SkillName.Wrestling, 90.0);

            this.Fame = 0;
            this.Karma = 0;

            this.VirtualArmor = 40;
            this.ControlSlots = (Core.SE) ? 2 : 1;
        }

        public BladeSpirits(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return Core.AOS;
            }
        }
        public override bool IsHouseSummonable
        {
            get
            {
                return true;
            }
        }
        public override double DispelDifficulty
        {
            get
            {
                return 0.0;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 20.0;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override double GetFightModeRanking(Mobile m, FightMode acqType, bool bPlayerOnly)
        {
            return (m.Str + m.Skills[SkillName.Tactics].Value) / Math.Max(this.GetDistanceToSqrt(m), 1.0);
        }

        public override int GetAngerSound()
        {
            return 0x23A;
        }

        public override int GetAttackSound()
        {
            return 0x3B8;
        }

        public override int GetHurtSound()
        {
            return 0x23A;
        }

        public override void OnThink()
        {
            if (Core.SE && this.Summoned)
            {
                ArrayList spirtsOrVortexes = new ArrayList();
                IPooledEnumerable eable = GetMobilesInRange(5);

                foreach (Mobile m in eable)
                {
                    if (m is EnergyVortex || m is BladeSpirits)
                    {
                        if (((BaseCreature)m).Summoned)
                            spirtsOrVortexes.Add(m);
                    }
                }

                eable.Free();

                while (spirtsOrVortexes.Count > 6)
                {
                    int index = Utility.Random(spirtsOrVortexes.Count);
                    this.Dispel(((Mobile)spirtsOrVortexes[index]));
                    spirtsOrVortexes.RemoveAt(index);
                }
            }

            base.OnThink();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}