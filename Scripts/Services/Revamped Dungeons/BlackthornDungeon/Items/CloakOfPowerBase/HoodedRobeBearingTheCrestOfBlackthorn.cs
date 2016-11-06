using Server;
using System;

namespace Server.Items
{
    [Flipable(0x2684, 0x2683)]
    public class HoodedRobeBearingTheCrestOfBlackthorn4 : BaseOuterTorso
    {        
        public override int LabelNumber { get { return 1029863; } } // Hooded Robe
        public override bool IsArtifact { get { return true; } }        
        
        [Constructable]
        public HoodedRobeBearingTheCrestOfBlackthorn4()
            : base(0x2683)
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
            Hue = 2107;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public HoodedRobeBearingTheCrestOfBlackthorn4(Serial serial)
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