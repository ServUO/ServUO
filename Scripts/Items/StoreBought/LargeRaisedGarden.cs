using System;
using Server.Gumps;

namespace Server.Items
{
    public class LargeRaisedGardenAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new LargeRaisedGardenDeed(); } }

        [Constructable]
        public LargeRaisedGardenAddon()
        {
            AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
            AddComponent(new GardenAddonComponent(19240), 1, 0, 0);
            AddComponent(new GardenAddonComponent(19235), 2, 0, 0);
            AddComponent(new GardenAddonComponent(19237), 2, 1, 0);
            AddComponent(new GardenAddonComponent(19239), 2, 2, 0);
            AddComponent(new GardenAddonComponent(19242), 1, 2, 0);
            AddComponent(new GardenAddonComponent(19238), 0, 2, 0);
            AddComponent(new GardenAddonComponent(19236), 0, 1, 0);
            AddComponent(new GardenAddonComponent(19241), 1, 1, 0);
        }

        public LargeRaisedGardenAddon(Serial serial)
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

    public class LargeRaisedGardenDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeRaisedGardenAddon(); } }

        [Constructable]
        public LargeRaisedGardenDeed()
        {
            LootType = LootType.Blessed;
        }

        public LargeRaisedGardenDeed(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1150651); // * Requires the "Rustic" theme pack
        }

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
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
