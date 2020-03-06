using System;

namespace Server.Items
{
    public class SingingBallOfSosarianSteeds : SingingBall
    {
        public override int LabelNumber { get { return 1152326; } } // singing ball of sosarian steeds
        
        [Constructable]
        public SingingBallOfSosarianSteeds()
            : base(0x468A)
        {
            Weight = 1.0;
            Hue = 2554;
            LootType = LootType.Regular;
        }

        public override int SoundList()
        {
            return Utility.RandomList(1218, 751, 629, 1226, 1305, 1246, 1019, 1508, 674, 1241);
        }

        public SingingBallOfSosarianSteeds(Serial serial)
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
