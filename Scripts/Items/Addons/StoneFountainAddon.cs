using System;

namespace Server.Items
{
    public class StoneFountainAddon : BaseAddon
    {
        [Constructable]
        public StoneFountainAddon()
        {
            int itemID = 0x1731;

            this.AddComponent(new AddonComponent(itemID++), -2, +1, 0);
            this.AddComponent(new AddonComponent(itemID++), -1, +1, 0);
            this.AddComponent(new AddonComponent(itemID++), +0, +1, 0);
            this.AddComponent(new AddonComponent(itemID++), +1, +1, 0);

            this.AddComponent(new AddonComponent(itemID++), +1, +0, 0);
            this.AddComponent(new AddonComponent(itemID++), +1, -1, 0);
            this.AddComponent(new AddonComponent(itemID++), +1, -2, 0);

            this.AddComponent(new AddonComponent(itemID++), +0, -2, 0);
            this.AddComponent(new AddonComponent(itemID++), +0, -1, 0);
            this.AddComponent(new AddonComponent(itemID++), +0, +0, 0);

            this.AddComponent(new AddonComponent(itemID++), -1, +0, 0);
            this.AddComponent(new AddonComponent(itemID++), -2, +0, 0);

            this.AddComponent(new AddonComponent(itemID++), -2, -1, 0);
            this.AddComponent(new AddonComponent(itemID++), -1, -1, 0);

            this.AddComponent(new AddonComponent(itemID++), -1, -2, 0);
            this.AddComponent(new AddonComponent(++itemID), -2, -2, 0);
        }

        public StoneFountainAddon(Serial serial)
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