using Server;
using System;

namespace Server.Items
{
    public class BascinetBearingTheCrestOfBlackthorn3 : Bascinet
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BascinetBearingTheCrestOfBlackthorn3()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.NightSight = 1;
            Attributes.BonusStr = 8;
            Attributes.DefendChance = 15;
            StrRequirement = 45;
            Hue = 1150;
        }

        public override int BasePhysicalResistance { get { return 20; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 10; } }
        public override int BasePoisonResistance { get { return 10; } }
        public override int BaseEnergyResistance { get { return 10; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }        

        public BascinetBearingTheCrestOfBlackthorn3(Serial serial)
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