using Server.Engines.Craft;
namespace Server.Items
{
    public class FletchingStationSouthAddon : SpecialVeteranCraftAddon
    {
        [Constructable]
        public FletchingStationSouthAddon()
            : this(0)
        { }

        [Constructable]
        public FletchingStationSouthAddon(int uses)
            : base(uses)
        {
            this.AddComponent(new FletchingStationComponent(), 0, 0, 0);
            this.AddComponent(new FletchingStationBoxComponent(), -1, 0, 0);
        }

        public FletchingStationSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override Direction AddonDirection
        {
            get
            {
                return Direction.South;
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
                return new FletchingStationSouthDeed(UsesRemaining);
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
    public class FletchingStationSouthDeed : BaseAddonDeed
    {
        private int storedUses;
        [Constructable]
        public FletchingStationSouthDeed()
            : this(0)
        {

        }
        public FletchingStationSouthDeed(int uses)
        {
            storedUses = uses;
            Name = "fletching station (south)";
        }

        public FletchingStationSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new FletchingStationSouthAddon(storedUses);
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