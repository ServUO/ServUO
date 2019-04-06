using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a deep sea serpents corpse")]
    public class DeepSeaSerpent : BaseCreature
    {
        [Constructable]
        public DeepSeaSerpent()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a deep sea serpent";
            Body = 150;
            BaseSoundID = 447;

            Hue = Utility.Random(0x8A0, 5);

            SetStr(251, 425);
            SetDex(87, 135);
            SetInt(87, 155);

            SetHits(151, 255);

            SetDamage(6, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 15, 20);

            SetSkill(SkillName.MagicResist, 60.1, 75.0);
            SetSkill(SkillName.Tactics, 60.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 70.0);

            Fame = 6000;
            Karma = -6000;

            VirtualArmor = 60;
            CanSwim = true;
            CantWalk = true;

            if (Utility.RandomBool())
                PackItem(new SulfurousAsh(4));
            else
                PackItem(new BlackPearl(4));
        }

        public DeepSeaSerpent(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel { get { return 2; } }
        public override bool HasBreath { get { return true; } }
        public override int Meat { get { return 10; } }
		public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override int Scales { get { return 8; } }
        public override ScaleType ScaleType { get { return ScaleType.Blue; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
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
