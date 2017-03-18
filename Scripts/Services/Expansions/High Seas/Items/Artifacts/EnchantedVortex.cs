using Server;
using System;

namespace Server.Items
{
    public class EnchantedVortexAddon : BaseAddon
    {

        public override BaseAddonDeed Deed { get { return new EnchantedVortexDeed(); } }

        [Constructable]
        public EnchantedVortexAddon()
        {
            AddonComponent comp = new AddonComponent(14284);
            comp.Name = "an enchanted vortex";
            AddComponent(comp, 0, 0, 0);
        }

        public EnchantedVortexAddon(Serial serial)
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

    public class EnchantedVortexDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new EnchantedVortexAddon(); } }

        [Constructable]
        public EnchantedVortexDeed()
        {
            Name = "A deed for an enchanted vortex";
        }

        public EnchantedVortexDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

    }
}
