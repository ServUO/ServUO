using System;
using Server.Gumps;

namespace Server.Items
{
    public class DecorativeBlackwidowAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new DecorativeBlackwidowDeed(); } }

        [Constructable]
        public DecorativeBlackwidowAddon()
        {
            AddComponent(new AddonComponent(40360), 1, 1, 0);
            AddComponent(new AddonComponent(40362), 1, 0, 0);
            AddComponent(new AddonComponent(40361), 0, 1, 0);
            AddComponent(new AddonComponent(40363), 0, 0, 0);
        }

        public DecorativeBlackwidowAddon(Serial serial)
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

    public class DecorativeBlackwidowDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new DecorativeBlackwidowAddon(); } }
        public override int LabelNumber { get { return 1157897; } }

        [Constructable]
        public DecorativeBlackwidowDeed()
        {
        }

        public DecorativeBlackwidowDeed(Serial serial)
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
