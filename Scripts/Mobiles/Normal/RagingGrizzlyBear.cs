using System;

namespace Server.Mobiles
{
    [CorpseName("a grizzly bear corpse")]
    public class RagingGrizzlyBear : BaseCreature
    {
        [Constructable]
        public RagingGrizzlyBear()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a raging grizzly bear";
            this.Body = 212;
            this.BaseSoundID = 0xA3;

            this.SetStr(1251, 1550);
            this.SetDex(801, 1050);
            this.SetInt(151, 400);

            this.SetHits(751, 930);
            this.SetMana(0);

            this.SetDamage(18, 23);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 70);
            this.SetResistance(ResistanceType.Cold, 30, 50);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Wrestling, 73.4, 88.1);
            this.SetSkill(SkillName.Tactics, 73.6, 110.5);
            this.SetSkill(SkillName.MagicResist, 32.8, 54.6);
            this.SetSkill(SkillName.Anatomy, 0, 0);

            this.Fame = 10000;  //Guessing here
            this.Karma = 10000;  //Guessing here

            this.VirtualArmor = 24;

            this.Tamable = false;
        }

        public RagingGrizzlyBear(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override int Hides
        {
            get
            {
                return 32;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Bear;
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