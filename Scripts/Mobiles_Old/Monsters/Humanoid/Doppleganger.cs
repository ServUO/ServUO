using System;

namespace Server.Mobiles
{
    [CorpseName("a doppleganger corpse")]
    public class Doppleganger : BaseCreature
    {
        [Constructable]
        public Doppleganger()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a doppleganger";
            this.Body = 0x309;
            this.BaseSoundID = 0x451;

            this.SetStr(81, 110);
            this.SetDex(56, 75);
            this.SetInt(81, 105);

            this.SetHits(101, 120);

            this.SetDamage(8, 12);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 75.1, 85.0);
            this.SetSkill(SkillName.Tactics, 70.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 90.0);

            this.Fame = 1000;
            this.Karma = -1000;

            this.VirtualArmor = 55;
        }

        public Doppleganger(Serial serial)
            : base(serial)
        {
        }

        public override int Hides
        {
            get
            {
                return 6;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
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