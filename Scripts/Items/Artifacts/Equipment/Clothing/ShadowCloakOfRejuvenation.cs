using System;
using Server;

namespace Server.Items
{
    public class ShadowCloakOfRejuvenation : Cloak
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115649; } } // Shadow Cloak Of Rejuvenation

        [Constructable]
        public ShadowCloakOfRejuvenation()
        {
            this.Hue = 1884;

            this.Attributes.RegenMana = 1;
            this.Attributes.RegenHits = 1;
            this.Attributes.RegenStam = 1;
            this.Attributes.LowerManaCost = 2;
        }

        public ShadowCloakOfRejuvenation(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}