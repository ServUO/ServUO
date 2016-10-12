using Server;
using System;

namespace Server.Items
{
    public class TricorneHatBearingTheCrestOfBlackthorn1 : TricorneHat
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TricorneHatBearingTheCrestOfBlackthorn1()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.NightSight = 1;
            Attributes.DefendChance = 10;
            Attributes.CastRecovery = 3;
            Hue = 1233;
        }

        public override int BasePhysicalResistance { get { return 10; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 10; } }
        public override int BasePoisonResistance { get { return 10; } }
        public override int BaseEnergyResistance { get { return 10; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
       
        public TricorneHatBearingTheCrestOfBlackthorn1(Serial serial)
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