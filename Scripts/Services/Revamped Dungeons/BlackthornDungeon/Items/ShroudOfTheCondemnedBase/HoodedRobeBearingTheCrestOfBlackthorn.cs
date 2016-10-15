using Server;
using System;

namespace Server.Items
{
    [Flipable(0x2684, 0x2683)]
    public class HoodedRobeBearingTheCrestOfBlackthorn1 : BaseOuterTorso
    {        
        public override int LabelNumber { get { return 1029863; } } // Hooded Robe
        public override bool IsArtifact { get { return true; } }        
        
        [Constructable]
        public HoodedRobeBearingTheCrestOfBlackthorn1()
            : base(0x2683)
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusHits = 3;
            Attributes.BonusInt = 5;
            Hue = 2075;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public HoodedRobeBearingTheCrestOfBlackthorn1(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}