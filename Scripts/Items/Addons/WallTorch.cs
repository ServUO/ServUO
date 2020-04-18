using Server.Network;

namespace Server.Items
{
    [Flipable(0x3D98, 0x3D94)]
    public class WallTorchComponent : AddonComponent
    {
        public WallTorchComponent()
            : base(0x3D98)
        {
        }

        public WallTorchComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1076282;// Wall Torch
        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                switch (ItemID)
                {
                    case 0x3D98:
                        ItemID = 0x3D9B;
                        break;
                    case 0x3D9B:
                        ItemID = 0x3D98;
                        break;
                    case 0x3D94:
                        ItemID = 0x3D97;
                        break;
                    case 0x3D97:
                        ItemID = 0x3D94;
                        break;
                }

                Effects.PlaySound(Location, Map, 0x3BE);
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
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

    public class WallTorchAddon : BaseAddon
    {
        public WallTorchAddon()
            : base()
        {
            AddComponent(new WallTorchComponent(), 0, 0, 0);
        }

        public WallTorchAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new WallTorchDeed();
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

    public class WallTorchDeed : BaseAddonDeed
    {
        [Constructable]
        public WallTorchDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public WallTorchDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new WallTorchAddon();
        public override int LabelNumber => 1076282;// Wall Torch
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