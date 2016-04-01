using System;

namespace Server.Mobiles
{
    [CorpseName("the remains of an enraged colossus")]
    public class EnragedColossus : BaseCreature
    {
        [Constructable]
        public EnragedColossus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.5)
        {
            this.Name = "Rising Colossus";
            this.Body = 829;

            this.SetStr(600);
            this.SetDex(70);
            this.SetInt(80);

            this.SetDamage(18, 21);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 56, 63);
            this.SetResistance(ResistanceType.Fire, 29, 30);
            this.SetResistance(ResistanceType.Cold, 53, 54);
            this.SetResistance(ResistanceType.Poison, 54, 58);
            this.SetResistance(ResistanceType.Energy, 26, 29);

            this.SetSkill(SkillName.MagicResist, 115.0, 140.0);
            this.SetSkill(SkillName.Tactics, 120.0, 130.0);
            this.SetSkill(SkillName.Wrestling, 120.0, 140);

            this.VirtualArmor = 58;
            this.ControlSlots = 5;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 4);
            this.AddLoot(LootPack.Gems, 8);
        }

        public EnragedColossus(Serial serial)
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