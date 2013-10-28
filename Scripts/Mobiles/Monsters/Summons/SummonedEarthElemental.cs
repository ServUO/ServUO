using System;

namespace Server.Mobiles
{
    [CorpseName("an earth elemental corpse")]
    public class SummonedEarthElemental : BaseCreature
    {
        [Constructable]
        public SummonedEarthElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an earth elemental";
            this.Body = 14;
            this.BaseSoundID = 268;

            this.SetStr(200);
            this.SetDex(70);
            this.SetInt(70);

            this.SetHits(180);

            this.SetDamage(14, 21);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.MagicResist, 65.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.0);

            this.VirtualArmor = 34;
            this.ControlSlots = 2;
        }

        public SummonedEarthElemental(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty
        {
            get
            {
                return 117.5;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 45.0;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}