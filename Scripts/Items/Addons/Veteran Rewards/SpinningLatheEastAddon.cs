using Server.Engines.Craft;
namespace Server.Items
{
    public class SpinningLatheEastAddon : SpecialVeteranCraftAddon
    {
        [Constructable]
        public SpinningLatheEastAddon()
            : this(0)
        { }

        [Constructable]
        public SpinningLatheEastAddon(int uses)
            : base(uses)
        {
            this.AddComponent(new SpinningLatheComponent(), 0, 0, 0);
            this.AddComponent(new SpinningLatheBoxComponent(), 0, 1, 0);
        }

        public SpinningLatheEastAddon(Serial serial)
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
                return DefCarpentry.CraftSystem;
            }
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SpinningLatheEastDeed(UsesRemaining);
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
    public class SpinningLatheEastDeed : BaseAddonDeed
    {
        private int storedUses;
        [Constructable]
        public SpinningLatheEastDeed()
            : this(0)
        {

        }
        public SpinningLatheEastDeed(int uses)
        {
            storedUses = uses;
            Name = "spinning lathe (east)";
        }

        public SpinningLatheEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SpinningLatheEastAddon(storedUses);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1123986;
            }
        }// spinning lathe - in case the Name is reset or having the south there is not liked
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