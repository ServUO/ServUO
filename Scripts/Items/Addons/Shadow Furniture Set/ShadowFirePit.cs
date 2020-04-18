using Server.Network;

namespace Server.Items
{
    public class ShadowFirePitAddon : BaseAddon
    {
        [Constructable]
        public ShadowFirePitAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x3653, 1076680), 1, 0, 0);
            AddComponent(new LocalizedAddonComponent(0x3652, 1076680), 0, 1, 0);
            AddComponent(new LocalizedAddonComponent(0x3651, 1076680), 1, 1, 0);
        }

        public ShadowFirePitAddon(Serial serial)
            : base(serial)
        {
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(c.Location, 3))
            {
                if (c.ItemID >= 0x3651 && c.ItemID <= 0x3653)
                {
                    Components.ForEach(x => x.ItemID += 3);
                }
                else
                {
                    Components.ForEach(x => x.ItemID -= 3);
                }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public override BaseAddonDeed Deed => new ShadowFirePitDeed();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowFirePitDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1076680;  // Shadow Fire Pit

        public override bool ExcludeDeedHue => true;

        [Constructable]
        public ShadowFirePitDeed()
        {
            LootType = LootType.Blessed;
            Hue = 1908;
        }

        public ShadowFirePitDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ShadowFirePitAddon();


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
