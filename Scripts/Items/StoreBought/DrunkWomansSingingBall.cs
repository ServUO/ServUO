using System;

namespace Server.Items
{
    public class DrunkWomansSingingBall : SingingBall
    {
        public override int LabelNumber { get { return 1152323; } } // drunk woman's singing ball

        [Constructable]
        public DrunkWomansSingingBall()
            : base(0x468A)
        {
            Weight = 1.0;
            Hue = 2596;
            LootType = LootType.Regular;
        }

        public override int SoundList()
        {
            return Utility.RandomMinMax(778, 823);
        }

        public DrunkWomansSingingBall(Serial serial)
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
