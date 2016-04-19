using Server.Engines.Craft;
namespace Server.Items
{
    public class FletchingStationEastAddon : SpecialVeteranCraftAddon
    {
        [Constructable]
        public FletchingStationEastAddon()
            : this(0)
        { }

        [Constructable]
        public FletchingStationEastAddon(int uses)
            : base(uses)
        {
            this.AddComponent(new FletchingStationComponent(), 0, 0, 0);
            this.AddComponent(new FletchingStationBoxComponent(), 0, 1, 0);
        }

        public FletchingStationEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override Direction AddonDirection
        {
            get
            {
                return Direction.East;
            }
        }

        public override Engines.Craft.CraftSystem CraftSystem
        {
            get
            {
                return DefBowFletching.CraftSystem;
            }
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new FletchingStationEastDeed(UsesRemaining);
            }
        }
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
    public class FletchingStationEastDeed : BaseAddonDeed
    {
        private int storedUses;
        [Constructable]
        public FletchingStationEastDeed()
            : this(0)
        {

        }
        public FletchingStationEastDeed(int uses)
        {
            storedUses = uses;
            Name = "fletching station (east)";
        }

        public FletchingStationEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FletchingStationEastAddon(storedUses);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1124006;
            }
        }// fletching station - in case the Name is reset or having the south there is not liked
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)storedUses);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            storedUses = reader.ReadInt();
        }
    }
}