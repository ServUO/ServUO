using System;
using Server.Targeting;

namespace Server.Items
{
    public class ClothingBlessTarget : Target // Create our targeting class (which we derive from the base target class)
    {
        private readonly ClothingBlessDeed m_Deed;
        public ClothingBlessTarget(ClothingBlessDeed deed)
            : base(1, false, TargetFlags.None)
        {
            this.m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target) // Override the protected OnTarget() for our feature
        {
            if (this.m_Deed.Deleted || this.m_Deed.RootParent != from)
                return;

            if (target is BaseClothing)
            {
                BaseClothing item = (BaseClothing)target;

                if (item is IArcaneEquip)
                {
                    IArcaneEquip eq = (IArcaneEquip)item;
                    if (eq.IsArcane)
                    {
                        from.SendLocalizedMessage(1005019); // This bless deed is for Clothes only.
                        return;
                    }
                }

                if (item.LootType == LootType.Blessed || item.BlessedFor == from || (Mobile.InsuranceEnabled && item.Insured)) // Check if its already newbied (blessed)
                {
                    from.SendLocalizedMessage(1045113); // That item is already blessed
                }
                else if (item.LootType != LootType.Regular)
                {
                    from.SendLocalizedMessage(1045114); // You can not bless that item
                }
                else if (!item.CanBeBlessed || item.RootParent != from)
                {
                    from.SendLocalizedMessage(500509); // You cannot bless that object
                }
                else
                {
                    item.LootType = LootType.Blessed;
                    from.SendLocalizedMessage(1010026); // You bless the item....

                    this.m_Deed.Delete(); // Delete the bless deed
                }
            }
            else
            {
                from.SendLocalizedMessage(500509); // You cannot bless that object
            }
        }
    }

    public class ClothingBlessDeed : Item // Create the item class which is derived from the base item class
    {
        [Constructable]
        public ClothingBlessDeed()
            : base(0x14F0)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }

        public ClothingBlessDeed(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "a clothing bless deed";
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
            this.LootType = LootType.Blessed;

            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
        {
            if (!this.IsChildOf(from.Backpack)) // Make sure its in their pack
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.SendLocalizedMessage(1005018); // What would you like to bless? (Clothes Only)
                from.Target = new ClothingBlessTarget(this); // Call our target
            }
        }
    }
}