namespace Server.Items
{
    public class SmithingPressSouthAddon : SpecialVeteranCraftAddon
    {
        [Constructable]
        public SmithingPressSouthAddon()
            : this(0)
        { }

        [Constructable]
        public SmithingPressSouthAddon(int uses)
            : base(uses)
        {
            this.AddComponent(new SmithingPressComponent(), 0, 0, 0); // 0x9AA9
            this.AddComponent(new SmithingPressBoxComponent(), 1, -1, 0);
        }

        public SmithingPressSouthAddon(Serial serial)
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

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SmithingPressSouthDeed(UsesRemaining);
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
    public class SmithingPressSouthDeed : BaseAddonDeed
    {
        private int storedUses;
        [Constructable]
        public SmithingPressSouthDeed()
            : this(0)
        {

        }
        public SmithingPressSouthDeed(int uses)
        {
            storedUses = uses;
            Name = "smithing press (south)";
        }

        public SmithingPressSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SmithingPressSouthAddon(storedUses);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1123577;
            }
        }// smithing press - in case the Name is reset or having the south there is not liked
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