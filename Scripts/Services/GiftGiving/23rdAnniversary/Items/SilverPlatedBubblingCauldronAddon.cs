using System.Linq;

namespace Server.Items
{
    public class AddonComponentHue : AddonComponent
    {
        public AddonComponentHue(int hue)
            : base(0xA5B5)
        {
            Hue = hue;
        }

        public AddonComponentHue(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class SilverPlatedBubblingCauldronAddon : BaseAddon
    {
        [Constructable]
        public SilverPlatedBubblingCauldronAddon(int hue)
        {
            AddComponent(new AddonComponent(0xA5B4), 0, 0, 0);
            AddComponent(new AddonComponentHue(hue), 0, 0, 0);
        }

        public SilverPlatedBubblingCauldronAddon(Serial serial)
            : base(serial)
        {
        }

        public override bool RetainComponentHue => true;

        public override BaseAddonDeed Deed => new SilverPlatedBubblingCauldronDeed(Components.OfType<AddonComponentHue>().FirstOrDefault().Hue);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class SilverPlatedBubblingCauldronDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1159508; // Silver Plated Bubbling Cauldron

        public override bool ExcludeDeedHue => true;

        [Constructable]
        public SilverPlatedBubblingCauldronDeed()
            : this(Utility.RandomMinMax(18, 224))
        {
        }

        [Constructable]
        public SilverPlatedBubblingCauldronDeed(int hue)
        {
            Hue = hue;
            LootType = LootType.Blessed;
        }

        public SilverPlatedBubblingCauldronDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new SilverPlatedBubblingCauldronAddon(Hue);
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
