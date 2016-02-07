using System;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x2A75, 0x2A76)]
    public class MountedPixieBlueComponent : AddonComponent
    {
        public MountedPixieBlueComponent()
            : base(0x2A75)
        {
        }

        public MountedPixieBlueComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074482;
            }
        }// Mounted pixie
        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.InRange(this.Location, from.Location, 2))
                Effects.PlaySound(this.Location, this.Map, Utility.RandomMinMax(0x55C, 0x55E));
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

    public class MountedPixieBlueAddon : BaseAddon
    {
        public MountedPixieBlueAddon()
            : base()
        {
            this.AddComponent(new MountedPixieBlueComponent(), 0, 0, 0);
        }

        public MountedPixieBlueAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new MountedPixieBlueDeed();
            }
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

    public class MountedPixieBlueDeed : BaseAddonDeed
    {
        [Constructable]
        public MountedPixieBlueDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public MountedPixieBlueDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new MountedPixieBlueAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074482;
            }
        }// Mounted pixie
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