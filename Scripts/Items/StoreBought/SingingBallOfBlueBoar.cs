using System;

namespace Server.Items
{
    public class SingingBallOfBlueBoar : SingingBall
    {
        public override int LabelNumber { get { return 1152327; } } // singing ball of the blue boar
        
        [Constructable]
        public SingingBallOfBlueBoar()
            : base(0x468A)
        {
            Weight = 1.0;
            Hue = 2514;
            LootType = LootType.Regular;
        }

        public override int SoundList()
        {
            return Utility.RandomList(1073, 1085, 811, 799, 1066, 794, 801, 1075, 803, 811, 1071);
        }

        public SingingBallOfBlueBoar(Serial serial)
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
