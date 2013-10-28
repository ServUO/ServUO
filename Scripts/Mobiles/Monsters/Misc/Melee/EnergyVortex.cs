using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("an energy vortex corpse")]
    public class EnergyVortex : BaseCreature
    {
        [Constructable]
        public EnergyVortex()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an energy vortex";

            if (Core.SE && 0.002 > Utility.RandomDouble()) // Per OSI FoF, it's a 1/500 chance.
            {
                // Llama vortex!
                this.Body = 0xDC;
                this.Hue = 0x76;
            }
            else
            {
                this.Body = 164;
            }

            this.SetStr(200);
            this.SetDex(200);
            this.SetInt(100);

            this.SetHits((Core.SE) ? 140 : 70);
            this.SetStam(250);
            this.SetMana(0);

            this.SetDamage(14, 17);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Energy, 100);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 90, 100);

            this.SetSkill(SkillName.MagicResist, 99.9);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 120.0);

            this.Fame = 0;
            this.Karma = 0;

            this.VirtualArmor = 40;
            this.ControlSlots = (Core.SE) ? 2 : 1;
        }

        public EnergyVortex(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return this.Summoned;
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
            return (m.Int + m.Skills[SkillName.Magery].Value) / Math.Max(this.GetDistanceToSqrt(m), 1.0);
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
            if (Core.SE && this.Summoned)
            {
                ArrayList spirtsOrVortexes = new ArrayList();

                foreach (Mobile m in this.GetMobilesInRange(5))
                {
                    if (m is EnergyVortex || m is BladeSpirits)
                    {
                        if (((BaseCreature)m).Summoned)
                            spirtsOrVortexes.Add(m);
                    }
                }

                while (spirtsOrVortexes.Count > 6)
                {
                    int index = Utility.Random(spirtsOrVortexes.Count);
                    //TODO: Confim if it's the dispel with all the pretty effects or just a Deletion of it.
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

            if (this.BaseSoundID == 263)
                this.BaseSoundID = 0;
        }
    }
}