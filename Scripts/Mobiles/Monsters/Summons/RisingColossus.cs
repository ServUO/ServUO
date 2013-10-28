using System;

namespace Server.Mobiles
{
    [CorpseName("the remains of a rising colossus")]
    public class RisingColossus : BaseCreature
    {
        [Constructable]
        public RisingColossus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.5)
        {
            this.Name = "Rising Colossus";
            this.Body = 829;

            this.SetStr(100);
            this.SetDex(100);
            this.SetInt(100);

            this.SetDamage(17, 25);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 100, 105);
            this.SetResistance(ResistanceType.Fire, 100, 105);
            this.SetResistance(ResistanceType.Cold, 100, 105);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);

            this.VirtualArmor = 58;
            this.ControlSlots = 5;
        }

        public RisingColossus(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty
        {
            get
            {
                return 125.0;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 45.0;
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
        }// Immune to poison?
        public override int GetAttackSound()
        {
            return 0x627;
        }

        public override int GetHurtSound()
        {
            return 0x629;
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