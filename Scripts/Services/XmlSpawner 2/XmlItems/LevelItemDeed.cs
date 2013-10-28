// By SHAMBAMPOW
using System;
using Server.Engines.XmlSpawner2;
using Server.Targeting;

namespace Server.Items
{
    public class LevelItemTarget : Target 
    {
        private readonly LevelItemDeed m_Deed;
        public LevelItemTarget(LevelItemDeed deed)
            : base(1, false, TargetFlags.None)
        {
            this.m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is BaseWeapon || target is BaseArmor || target is BaseClothing || target is BaseJewel)
            {
                Item item = (Item)target;

                XmlLevelItem a = (XmlLevelItem)XmlAttach.FindAttachment(item, typeof(XmlLevelItem));

                if (a != null)
                {
                    from.SendMessage("That already is levelable!");
                    return;
                }
                else
                {
                    if (item.RootParent != from) // Make sure its in their pack or they are wearing it
                    {
                        from.SendMessage("You cannot make that levelable there!");
                    }
                    else
                    {
                        // mod to attach the XmlPoints attachment automatically to new chars
                        XmlAttach.AttachTo(item, new XmlLevelItem());

                        from.SendMessage("You magically make the item levelable...");

                        this.m_Deed.Delete();
                    }
                }
            }
            else
            {
                from.SendMessage("You cannot make that levelable");
            }
        }
    }

    public class LevelItemDeed : Item // Create the item class which is derived from the base item class
    {
        [Constructable]
        public LevelItemDeed()
            : base(0x14F0)
        {
            this.Weight = 1.0;
            this.Name = "a levelable item deed";
            this.LootType = LootType.Blessed;
            this.Hue = 1171;
        }

        public LevelItemDeed(Serial serial)
            : base(serial)
        {
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
                from.SendMessage("What item would you like to make levelable?");
                from.Target = new LevelItemTarget(this); // Call our target
            }
        }
    }
}