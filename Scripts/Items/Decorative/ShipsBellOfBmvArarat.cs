using System;
using Server.Items;

namespace Server.Items
{
    public class ShipsBellOfBmvArarat : BaseDecorationArtifact
    {
        public override int ArtifactRarity { get { return 8; } }
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ShipsBellOfBmvArarat()
            : base(0x4C5E)
        {
            this.Name = "Ship's Bell Of The Bmv Ararat";
            this.Weight = 10.0;
            this.Hue = 2968; //checked
        }

        public ShipsBellOfBmvArarat(Serial serial)
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