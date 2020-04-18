using Server.Engines.Distillation;
using Server.Multis;

namespace Server.Items
{
    public class DistilleryEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new DistilleryEastAddonDeed();

        [Constructable]
        public DistilleryEastAddon()
        {
            AddComponent(new LocalizedAddonComponent(15802, 1150640), 0, 0, 0);
            AddComponent(new LocalizedAddonComponent(15803, 1150640), 0, 1, 0);
        }

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(component);

            if (from.InRange(component.Location, 2) && (house == null || house.IsCoOwner(from)))
                from.SendGump(new DistillationGump(from));
        }

        public DistilleryEastAddon(Serial serial)
            : base(serial)
        {
        }

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

    public class DistilleryEastAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new DistilleryEastAddon();
        public override int LabelNumber => 1150664;  // distillery (east)

        [Constructable]
        public DistilleryEastAddonDeed()
        {
        }

        public DistilleryEastAddonDeed(Serial serial)
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

    public class DistillerySouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new DistillerySouthAddonDeed();

        [Constructable]
        public DistillerySouthAddon()
        {
            AddComponent(new LocalizedAddonComponent(15800, 1150640), 0, 0, 0);
            AddComponent(new LocalizedAddonComponent(15801, 1150640), -1, 0, 0);
        }

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(component);

            if (house == null || house.IsCoOwner(from))
                from.SendGump(new DistillationGump(from));
        }

        public DistillerySouthAddon(Serial serial)
            : base(serial)
        {
        }

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

    public class DistillerySouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new DistillerySouthAddon();
        public override int LabelNumber => 1150663;  // distillery (south)

        [Constructable]
        public DistillerySouthAddonDeed()
        {
        }

        public DistillerySouthAddonDeed(Serial serial)
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