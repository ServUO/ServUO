using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sea serpents corpse")]
    [TypeAlias("Server.Mobiles.Seaserpant")]
    public class SeaSerpent : BaseCreature
    {
        [Constructable]
        public SeaSerpent()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a sea serpent";
            this.Body = 150;
            this.BaseSoundID = 447;

            this.Hue = Utility.Random(0x530, 9);

            this.SetStr(168, 225);
            this.SetDex(58, 85);
            this.SetInt(53, 95);

            this.SetHits(110, 127);

            this.SetDamage(7, 13);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 15, 20);

            this.SetSkill(SkillName.MagicResist, 60.1, 75.0);
            this.SetSkill(SkillName.Tactics, 60.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 70.0);

            this.Fame = 6000;
            this.Karma = -6000;

            this.VirtualArmor = 30;
            this.CanSwim = true;
            this.CantWalk = true;

            if (Utility.RandomBool())
                this.PackItem(new SulfurousAsh(4));
            else
                this.PackItem(new BlackPearl(4));

            this.PackItem(new RawFishSteak());
            this.PackItem(new SpecialFishingNet());

            if (Utility.RandomDouble() < .05)
                this.PackItem(new MessageInABottle());
        }

        public SeaSerpent(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath { get { return true; } }
        public override int TreasureMapLevel { get { return Utility.RandomList(1, 2); } }
        public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Horned; } }
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