using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("an energy vortex corpse")]
    public class EnergyVortex : BaseCreature
    {
        [Constructable]
        public EnergyVortex() : this(false)
        {
        }

        [Constructable]
        public EnergyVortex(bool summoned)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an energy vortex";

            if (Core.SE && 0.002 > Utility.RandomDouble()) // Per OSI FoF, it's a 1/500 chance.
            {
                // Llama vortex!
                Body = 0xDC;
                Hue = 0x76;
            }
            else
            {
                Body = 164;
            }

            bool weak = summoned && Siege.SiegeShard;

            SetStr(weak ? 100 : 200);
            SetDex(weak ? 150 : 200);
            SetInt(100);

            SetHits((Core.SE && !weak) ? 140 : 70);
            SetStam(250);
            SetMana(0);

            SetDamage(weak ? 10 : 14, weak ? 13 : 17);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Energy, 100);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 90, 100);

            SetSkill(SkillName.MagicResist, 99.9);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 120.0);

            Fame = 0;
            Karma = 0;

            VirtualArmor = 40;
            ControlSlots = (Core.SE) ? 2 : 1;
        }

        public EnergyVortex(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return Summoned;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }// Or Llama vortices will appear gray.
        public override double DispelDifficulty
        {
            get
            {
                return 80.0;
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
            return (m.Int + m.Skills[SkillName.Magery].Value) / Math.Max(GetDistanceToSqrt(m), 1.0);
        }

        public override int GetAngerSound()
        {
            return 0x15;
        }

        public override int GetAttackSound()
        {
            return 0x28;
        }

        public override void OnThink()
        {
            if (Core.SE && Summoned)
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
                    //TODO: Confim if it's the dispel with all the pretty effects or just a Deletion of it.
                    Dispel(((Mobile)spirtsOrVortexes[index]));
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

            if (BaseSoundID == 263)
                BaseSoundID = 0;
        }
    }
}