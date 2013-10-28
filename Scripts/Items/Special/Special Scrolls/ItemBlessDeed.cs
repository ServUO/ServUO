using System;
using Server.Targeting;

namespace Server.Items
{
    public class ItemBlessTarget : Target // Create our targeting class (which we derive from the base target class)
    {
        private readonly ItemBlessDeed m_Deed;
        public ItemBlessTarget(ItemBlessDeed deed)
            : base(1, false, TargetFlags.None)
        {
            this.m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target) // Override the protected OnTarget() for our feature
        {
            if (this.m_Deed.Deleted || this.m_Deed.RootParent != from)
                return;

            if (target is Item)
            {
                Item item = (Item)target;

                if (item.RootParent != from) // Make sure its in their pack or they are wearing it
                    from.SendLocalizedMessage(500508); // You may only bless objects that you are carrying.
                else if (item.Stackable == true)
                {
                    from.SendLocalizedMessage(500509); // You cannot bless that object
                }
                else if (item is Key || item is BaseContainer)
                {
                    from.SendLocalizedMessage(500509); // You cannot bless that object
                }
                else if (item.LootType == LootType.Blessed || item.BlessedFor == from || (Mobile.InsuranceEnabled && item.Insured)) // Check if its already newbied (blessed)
                {
                    from.SendLocalizedMessage(1045113); // That item is already blessed
                }
                else if (item.LootType != LootType.Regular)
                {
                    from.SendLocalizedMessage(1045114); // You can not bless that item
                }
                else
                {
                    item.LootType = LootType.Blessed;
                    from.SendLocalizedMessage(1075281); // Your item has been blessed

                    this.m_Deed.Delete(); // Delete the bless deed
                }
            }
            else
            {
                from.SendLocalizedMessage(500509); // You cannot bless that object
            }
        }
    }

    public class ItemBlessDeed : Item // Create the item class which is derived from the base item class
    {
        [Constructable]
        public ItemBlessDeed()
            : base(0x14F0)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }

        public ItemBlessDeed(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "a item bless deed";
            }
        }
        public override bool DisplayLootType
        {
            get
            {
                return false;
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

            this.LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
        {
            if (!this.IsChildOf(from.Backpack)) // Make sure its in their pack
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.SendLocalizedMessage(500506); // Which item do you wish to bless?
                from.Target = new ItemBlessTarget(this); // Call our target
            }
        }
    }
}