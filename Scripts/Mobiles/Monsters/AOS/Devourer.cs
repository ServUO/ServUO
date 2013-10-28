using System;

namespace Server.Mobiles
{
    [CorpseName("a devourer of souls corpse")]
    public class Devourer : BaseCreature
    {
        [Constructable]
        public Devourer()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a devourer of souls";
            this.Body = 303;
            this.BaseSoundID = 357;

            this.SetStr(801, 950);
            this.SetDex(126, 175);
            this.SetInt(201, 250);

            this.SetHits(650);

            this.SetDamage(22, 26);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.0);
            this.SetSkill(SkillName.Meditation, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 90.1, 105.0);
            this.SetSkill(SkillName.Tactics, 75.1, 85.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 100.0);

            this.Fame = 9500;
            this.Karma = -9500;

            this.VirtualArmor = 44;

            this.PackNecroReg(24, 45);
        }

        public Devourer(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int Meat
        {
            get
            {
                return 3;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
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