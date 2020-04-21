using Server.Gumps;
using Server.Targeting;

namespace Server.Items
{
    public class PersonalBlessDeed : Item
    {
        private Mobile m_Owner;
        [Constructable]
        public PersonalBlessDeed()
            : base(0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        [Constructable]
        public PersonalBlessDeed(Mobile owner)
            : base(0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Owner = owner;
            InvalidateProperties();
        }

        public PersonalBlessDeed(Serial serial)
            : base(serial)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override bool DisplayLootType => true;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return m_Owner;
            }
            set
            {
                m_Owner = value;
                InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            LootType = LootType.Blessed;

            int version = reader.ReadInt();

            m_Owner = reader.ReadMobile();
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            string m_name = "a player";

            if (m_Owner != null && !m_Owner.Deleted)
                m_name = m_Owner.Name;

            list.Add(1062195, m_name); // personal bless deed for ~1_NAME~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from != m_Owner)
                from.SendLocalizedMessage(1062201, m_Owner != null ? m_Owner.Name : "another player"); // Only the owner, ~1_NAME~, can use this deed.
            else if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            else
                from.SendGump(new WarningGump(1060635, 30720, 1062202, 32512, 420, 280, BlessWarning_Callback, this));
        }

        private static void BlessWarning_Callback(Mobile from, bool okay, object state)
        {
            if (!okay)
                return;

            from.SendLocalizedMessage(500506); // Which item do you wish to bless?
            from.Target = new PersonalBlessTarget((PersonalBlessDeed)state);
        }
    }

    public class PersonalBlessTarget : Target // Create targeting class
    {
        private readonly PersonalBlessDeed m_Deed;
        public PersonalBlessTarget(PersonalBlessDeed deed)
            : base(-1, false, TargetFlags.None)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is Mobile)
                from.SendLocalizedMessage(500507); // You can only bless an item!
            else if (target is BaseArmor || target is BaseClothing || target is BaseWeapon || target is BaseJewel)
            {
                Item item = (Item)target;

                if (item.LootType == LootType.Blessed || item.BlessedFor != null || (Mobile.InsuranceEnabled && item.Insured)) // Check if its already newbied (blessed)
                    from.SendLocalizedMessage(1045113); // That item is already blessed
                else if (item.LootType != LootType.Regular)
                    from.SendLocalizedMessage(500509); // You can't bless this object!
                else if (item.RootParent != from) // Make sure its in their pack or they are wearing it
                    from.SendLocalizedMessage(500508); // You may only bless objects that you are carrying.
                else if (m_Deed.RootParent != from)
                    from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                else
                {
                    // Now we also know the deed is still in the players backpack,
                    // as is the item the player wants to bless. Let's go and
                    // bless it.
                    if (item is BaseArmor)
                    {
                        BaseArmor mitem = (BaseArmor)item;
                        mitem.BlessedBy = from;
                    }
                    else if (item is BaseClothing)
                    {
                        BaseClothing mitem = (BaseClothing)item;
                        mitem.BlessedBy = from;
                    }
                    else if (item is BaseWeapon)
                    {
                        BaseWeapon mitem = (BaseWeapon)item;
                        mitem.BlessedBy = from;
                    }
                    else if (item is BaseJewel)
                    {
                        BaseJewel mitem = (BaseJewel)item;
                        mitem.BlessedBy = from;
                    }

                    item.BlessedFor = from;
                    from.SendLocalizedMessage(1062204); // You personally bless the item for this character.
                    from.PlaySound(0x202);
                    m_Deed.Delete();
                }
            }
            else
                from.SendLocalizedMessage(500509); // You can't bless this object!
        }
    }
}
