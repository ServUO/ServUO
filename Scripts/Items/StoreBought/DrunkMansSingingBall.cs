using System;

namespace Server.Items
{
    public class DrunkMansSingingBall : SingingBall
    {
        public override int LabelNumber { get { return 1152324; } } // drunk man's singing ball

        [Constructable]
        public DrunkMansSingingBall()
            : base(0x468A)
        {
            Weight = 1.0;
            Hue = 2659;
            LootType = LootType.Regular;
        }

        public override int SoundList()
        {
            return Utility.RandomMinMax(1049, 1098);
        }

        public DrunkMansSingingBall(Serial serial)
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
