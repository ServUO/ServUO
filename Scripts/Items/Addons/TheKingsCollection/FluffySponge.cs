using System;

namespace Server.Items
{
    public class FluffySpongeAddon : BaseAddon, IDyable
    {
        [Constructable]
        public FluffySpongeAddon()
        {
            AddComponent(new AddonComponent(0x4C31), 0, 0, 0);
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public FluffySpongeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new FluffySpongeDeed(); } }

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

    public class FluffySpongeDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1098377; } } // Fluffy Sponge

        [Constructable]
        public FluffySpongeDeed()
        {
            LootType = LootType.Blessed;
        }

        public FluffySpongeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new FluffySpongeAddon(); } }
        
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
