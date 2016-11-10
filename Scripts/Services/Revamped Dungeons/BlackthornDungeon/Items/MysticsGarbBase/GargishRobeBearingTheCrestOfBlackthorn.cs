using Server;
using System;

namespace Server.Items
{
    public class GargishRobeBearingTheCrestOfBlackthorn2 : GargishRobe
    {
        public override bool IsArtifact { get { return true; } }
        
        [Constructable]
        public GargishRobeBearingTheCrestOfBlackthorn2()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.LowerManaCost = 1;
            Attributes.BonusMana = 5;
            Hue = 1306;            
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public GargishRobeBearingTheCrestOfBlackthorn2(Serial serial)
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