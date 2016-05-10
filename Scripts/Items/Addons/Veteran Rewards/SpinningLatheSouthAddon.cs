using Server.Engines.Craft;
namespace Server.Items
{
    public class SpinningLatheSouthAddon : SpecialVeteranCraftAddon
    {
        [Constructable]
        public SpinningLatheSouthAddon()
            : this(0)
        { }

        [Constructable]
        public SpinningLatheSouthAddon(int uses)
            : base(uses)
        {
            this.AddComponent(new SpinningLatheComponent(), 0, 0, 0);
            this.AddComponent(new SpinningLatheBoxComponent(), -1, 0, 0);
        }

        public SpinningLatheSouthAddon(Serial serial)
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
                return DefCarpentry.CraftSystem;
            }
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SpinningLatheSouthDeed(UsesRemaining);
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
    public class SpinningLatheSouthDeed : BaseAddonDeed
    {
        private int storedUses;
        [Constructable]
        public SpinningLatheSouthDeed()
            : this(0)
        {

        }
        public SpinningLatheSouthDeed(int uses)
        {
            storedUses = uses;
            Name = "spinning lathe (south)";
        }

        public SpinningLatheSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SpinningLatheSouthAddon(storedUses);
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