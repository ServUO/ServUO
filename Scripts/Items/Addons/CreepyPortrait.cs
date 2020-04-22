using Server.Network;
using System;

namespace Server.Items
{
    [Flipable(0x2A69, 0x2A6D)]
    public class CreepyPortraitComponent : AddonComponent
    {
        public CreepyPortraitComponent()
            : base(0x2A69)
        {
        }

        public CreepyPortraitComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074481;// Creepy portrait
        public override bool HandlesOnMovement => true;
        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.InRange(Location, from.Location, 2))
                Effects.PlaySound(Location, Map, Utility.RandomMinMax(0x565, 0x566));
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void OnMovement(Mobile m, Point3D old)
        {
            if (m.Alive && m.Player && (m.IsPlayer() || !m.Hidden))
            {
                if (!Utility.InRange(old, Location, 2) && Utility.InRange(m.Location, Location, 2))
                {
                    if (ItemID == 0x2A69 || ItemID == 0x2A6D)
                    {
                        Up();
                        Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 2, Up);
                    }
                }
                else if (Utility.InRange(old, Location, 2) && !Utility.InRange(m.Location, Location, 2))
                {
                    if (ItemID == 0x2A6C || ItemID == 0x2A70)
                    {
                        Down();
                        Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 2, Down);
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0 && ItemID != 0x2A69 && ItemID != 0x2A6D)
                ItemID = 0x2A69;
        }

        private void Up()
        {
            ItemID += 1;
        }

        private void Down()
        {
            ItemID -= 1;
        }
    }

    public class CreepyPortraitAddon : BaseAddon
    {
        [Constructable]
        public CreepyPortraitAddon()
            : base()
        {
            AddComponent(new CreepyPortraitComponent(), 0, 0, 0);
        }

        public CreepyPortraitAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new CreepyPortraitDeed();
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

    public class CreepyPortraitDeed : BaseAddonDeed
    {
        [Constructable]
        public CreepyPortraitDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public CreepyPortraitDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new CreepyPortraitAddon();
        public override int LabelNumber => 1074481;// Creepy portrait
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
