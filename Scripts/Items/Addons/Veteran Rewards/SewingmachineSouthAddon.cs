using Server.Engines.Craft;
namespace Server.Items
{
    public class SewingMachineSouthAddon : SpecialVeteranCraftAddon
    {
        [Constructable]
        public SewingMachineSouthAddon()
            : this(0)
        { }

        [Constructable]
        public SewingMachineSouthAddon(int uses)
            : base(uses)
        {
            this.AddComponent(new SewingMachineComponent(), 0, 0, 0);
            this.AddComponent(new SewingMachineBoxComponent(), -1, 0, 0);
        }

        public SewingMachineSouthAddon(Serial serial)
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
                return DefTailoring.CraftSystem;
            }
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SewingMachineSouthDeed(UsesRemaining);
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
    public class SewingMachineSouthDeed : BaseAddonDeed
    {
        private int storedUses;
        [Constructable]
        public SewingMachineSouthDeed()
            : this(0)
        {

        }
        public SewingMachineSouthDeed(int uses)
        {
            storedUses = uses;
            Name = "sewing machine (south)";
        }

        public SewingMachineSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SewingMachineSouthAddon(storedUses);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1123504;
            }
        }// sewing machine - in case the Name is reset or having the south there is not liked
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