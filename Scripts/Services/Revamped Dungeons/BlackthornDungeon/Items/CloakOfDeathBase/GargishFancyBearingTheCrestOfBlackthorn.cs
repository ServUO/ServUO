using Server;
using System;

namespace Server.Items
{
    public class GargishFancyBearingTheCrestOfBlackthorn6 : GargishFancyRobe
    {
        public override bool IsArtifact { get { return true; } }
        
        [Constructable]
        public GargishFancyBearingTheCrestOfBlackthorn6()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.AttackChance = 3;
            Attributes.DefendChance = 3;
            Attributes.SpellDamage = 3;
            Hue = 2019;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public GargishFancyBearingTheCrestOfBlackthorn6(Serial serial)
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