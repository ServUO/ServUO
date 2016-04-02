using System;

namespace Server.Mobiles
{
    [CorpseName("a troll corpse")]
    public class Troll : BaseCreature
    {
        [Constructable]
        public Troll()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a troll";
            this.Body = Utility.RandomList(53, 54);
            this.BaseSoundID = 461;

            this.SetStr(176, 205);
            this.SetDex(46, 65);
            this.SetInt(46, 70);

            this.SetHits(106, 123);

            this.SetDamage(8, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 5, 15);
            this.SetResistance(ResistanceType.Energy, 5, 15);

            this.SetSkill(SkillName.MagicResist, 45.1, 60.0);
            this.SetSkill(SkillName.Tactics, 50.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 70.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 40;
        }

        public Troll(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override int Meat
        {
            get
            {
                return 2;
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