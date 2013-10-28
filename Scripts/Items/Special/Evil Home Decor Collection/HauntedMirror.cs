using System;

namespace Server.Items
{
    [Flipable(0x2A7B, 0x2A7D)]
    public class HaunterMirrorComponent : AddonComponent
    {
        public HaunterMirrorComponent()
            : base(0x2A7B)
        {
        }

        public HaunterMirrorComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074800;
            }
        }// Haunted Mirror
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public override void OnMovement(Mobile m, Point3D old)
        {
            base.OnMovement(m, old);

            if (m.Alive && m.Player && (m.IsPlayer() || !m.Hidden))
            {
                if (!Utility.InRange(old, this.Location, 2) && Utility.InRange(m.Location, this.Location, 2))
                {
                    if (this.ItemID == 0x2A7B || this.ItemID == 0x2A7D)
                    {
                        Effects.PlaySound(this.Location, this.Map, Utility.RandomMinMax(0x551, 0x553));
                        this.ItemID += 1;
                    }
                }
                else if (Utility.InRange(old, this.Location, 2) && !Utility.InRange(m.Location, this.Location, 2))
                {
                    if (this.ItemID == 0x2A7C || this.ItemID == 0x2A7E)
                        this.ItemID -= 1;
                }
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

    public class HaunterMirrorAddon : BaseAddon
    {
        [Constructable]
        public HaunterMirrorAddon()
            : base()
        {
            this.AddComponent(new HaunterMirrorComponent(), 0, 0, 0);
        }

        public HaunterMirrorAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new HaunterMirrorDeed();
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

    public class HaunterMirrorDeed : BaseAddonDeed
    {
        [Constructable]
        public HaunterMirrorDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public HaunterMirrorDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new HaunterMirrorAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074800;
            }
        }// Haunted Mirror
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