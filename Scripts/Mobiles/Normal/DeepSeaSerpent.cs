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
            this.Name = "a deep sea serpent";
            this.Body = 150;
            this.BaseSoundID = 447;

            this.Hue = Utility.Random(0x8A0, 5);

            this.SetStr(251, 425);
            this.SetDex(87, 135);
            this.SetInt(87, 155);

            this.SetHits(151, 255);

            this.SetDamage(6, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 15, 20);

            this.SetSkill(SkillName.MagicResist, 60.1, 75.0);
            this.SetSkill(SkillName.Tactics, 60.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 70.0);

            this.Fame = 6000;
            this.Karma = -6000;

            this.VirtualArmor = 60;
            this.CanSwim = true;
            this.CantWalk = true;

            if (Utility.RandomBool())
                this.PackItem(new SulfurousAsh(4));
            else
                this.PackItem(new BlackPearl(4));

            this.PackItem(new SpecialFishingNet());

            if (Utility.RandomDouble() < .05)
                this.PackItem(new MessageInABottle());
        }

        public DeepSeaSerpent(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel { get { return 2; } }
        public override bool HasBreath { get { return true; } }
        public override int Meat { get { return 1; } }
        public override int Scales { get { return 8; } }
        public override ScaleType ScaleType { get { return ScaleType.Blue; } }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
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