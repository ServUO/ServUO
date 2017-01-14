using Server;
using System;

namespace Server.Items
{
    public class GargishHalfApronBearingTheCrestOfBlackthorn2 : GargoyleHalfApron
    {
        public override bool IsArtifact { get { return true; } }
        
        [Constructable]
        public GargishHalfApronBearingTheCrestOfBlackthorn2()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 10;
            Attributes.RegenHits = 2;
            StrRequirement = 10;
            Hue = 1157;
        }   

        public GargishHalfApronBearingTheCrestOfBlackthorn2(Serial serial)
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