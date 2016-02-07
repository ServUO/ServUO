using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a frost troll corpse")]
    public class FrostTroll : BaseCreature
    {
        [Constructable]
        public FrostTroll()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a frost troll";
            this.Body = 55;
            this.BaseSoundID = 461;

            this.SetStr(227, 265);
            this.SetDex(66, 85);
            this.SetInt(46, 70);

            this.SetHits(140, 156);

            this.SetDamage(14, 20);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Cold, 25);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 65.1, 80.0);
            this.SetSkill(SkillName.Tactics, 80.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 100.0);

            this.Fame = 4000;
            this.Karma = -4000;

            this.VirtualArmor = 50;

            this.PackItem(new DoubleAxe()); // TODO: Weapon??
        }

        public FrostTroll(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 2;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Gems);
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