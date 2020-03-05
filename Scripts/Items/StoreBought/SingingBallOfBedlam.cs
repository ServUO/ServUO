using System;

namespace Server.Items
{
    public class SingingBallOfBedlam : SingingBall
    {
        public override int LabelNumber { get { return 1152325; } } // singing ball of bedlam
        
        [Constructable]
        public SingingBallOfBedlam()
            : base(0x468A)
        {
            Weight = 1.0;
            Hue = 2611;
            LootType = LootType.Regular;
        }

        public override int SoundList()
        {
            return Utility.RandomList(897, 1005, 889, 1001, 1002, 1004, 1005, 894, 893, 889, 1003);
        }

        public SingingBallOfBedlam(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
