using System;
using Server.Mobiles;

namespace Server.Items
{
    public class HeroOfTheUnlovedTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1157649; } } // Hero of the Unloved

        [Constructable]
        public HeroOfTheUnlovedTitleDeed()
        {
        }

        public HeroOfTheUnlovedTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class SaviorOfTheDementedTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1157651; } } // Savior of the Demented

        [Constructable]
        public SaviorOfTheDementedTitleDeed()
        {
        }

        public SaviorOfTheDementedTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class SlayerOfThePumpkinKingTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1157650; } } // Slayer of the Pumpkin King

        [Constructable]
        public SlayerOfThePumpkinKingTitleDeed()
        {
        }

        public SlayerOfThePumpkinKingTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}
