using System;

namespace Server.Items
{
    public class SkullPileAddon : BaseAddon
    {
        [Constructable]
        public SkullPileAddon()
        {
            this.AddComponent(new AddonComponent(6872), 1, 1, 0);
            this.AddComponent(new AddonComponent(6873), 0, 1, 0);
            this.AddComponent(new AddonComponent(6874), -1, 1, 0);
            this.AddComponent(new AddonComponent(6875), 0, 0, 0);
            this.AddComponent(new AddonComponent(6876), 1, 0, 0);
            this.AddComponent(new AddonComponent(6877), 1, -1, 0);
            this.AddComponent(new AddonComponent(6878), 2, -1, 0);
            this.AddComponent(new AddonComponent(6879), 2, 0, 0);
        }

        public SkullPileAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();
        }
    }
}