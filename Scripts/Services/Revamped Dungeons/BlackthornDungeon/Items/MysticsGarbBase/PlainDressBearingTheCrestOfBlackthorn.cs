using Server;
using System;

namespace Server.Items
{
    public class PlainDressBearingTheCrestOfBlackthorn2 : PlainDress
    {
        public override bool IsArtifact { get { return true; } }
        
        [Constructable]
        public PlainDressBearingTheCrestOfBlackthorn2()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.LowerManaCost = 1;
            Attributes.BonusMana = 5;
            Hue = 2075;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public PlainDressBearingTheCrestOfBlackthorn2(Serial serial)
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