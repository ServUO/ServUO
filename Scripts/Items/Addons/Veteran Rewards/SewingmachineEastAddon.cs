using Server.Engines.Craft;
namespace Server.Items
{
    public class SewingMachineEastAddon : SpecialVeteranCraftAddon
    {
        [Constructable]
        public SewingMachineEastAddon()
            : this(0)
        { }

        [Constructable]
        public SewingMachineEastAddon(int uses)
            : base(uses)
        {
            this.AddComponent(new SewingMachineComponent(), 0, 0, 0);
            this.AddComponent(new SewingMachineBoxComponent(), 0, -1, 0);
        }

        public SewingMachineEastAddon(Serial serial)
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
                return DefTailoring.CraftSystem;
            }
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SewingMachineEastDeed(UsesRemaining);
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
    public class SewingMachineEastDeed : BaseAddonDeed
    {
        private int storedUses;
        [Constructable]
        public SewingMachineEastDeed()
            : this(0)
        {

        }
        public SewingMachineEastDeed(int uses)
        {
            storedUses = uses;
            Name = "sewing machine (east)";
        }

        public SewingMachineEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SewingMachineEastAddon(storedUses);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1123504;
            }
        }// sewing machine - in case the Name is reset or having the east there is not liked
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