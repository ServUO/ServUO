using System;

namespace Server.Items
{
    public class ObsidianPillarAddon : BaseAddon
    {
        [Constructable]
        public ObsidianPillarAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x364F, 1076678), 0, 0, 0);
        }

        public ObsidianPillarAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new ObsidianPillarDeed(); } }

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

    public class ObsidianPillarDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1076678; } } // Obsidian Pillar

        public override bool ExcludeDeedHue { get { return true; } }

        [Constructable]
        public ObsidianPillarDeed()
        {
            LootType = LootType.Blessed;
            Hue = 1908;
        }

        public ObsidianPillarDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new ObsidianPillarAddon(); } }

        
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
