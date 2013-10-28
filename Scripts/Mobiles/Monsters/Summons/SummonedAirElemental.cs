using System;

namespace Server.Mobiles
{
    [CorpseName("an air elemental corpse")]
    public class SummonedAirElemental : BaseCreature
    {
        [Constructable]
        public SummonedAirElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an air elemental";
            this.Body = 13;
            this.Hue = 0x4001;
            this.BaseSoundID = 655;

            this.SetStr(200);
            this.SetDex(200);
            this.SetInt(100);

            this.SetHits(150);
            this.SetStam(50);

            this.SetDamage(6, 9);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 35, 45);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 70, 80);

            this.SetSkill(SkillName.Meditation, 90.0);
            this.SetSkill(SkillName.EvalInt, 70.0);
            this.SetSkill(SkillName.Magery, 70.0);
            this.SetSkill(SkillName.MagicResist, 60.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.0);

            this.VirtualArmor = 40;
            this.ControlSlots = 2;
        }

        public SummonedAirElemental(Serial serial)
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

            if (this.BaseSoundID == 263)
                this.BaseSoundID = 655;
        }
    }
}