using Server.Gumps;
using Server.Mobiles;
using System;

namespace Server.Items
{
    public class ValentineBear : Item, ICustomizableMessageItem, IFlipable
    {
        private string m_OwnerName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string OwnerName
        {
            get { return m_OwnerName; }
            set
            {
                m_OwnerName = value;
                InvalidateProperties();
            }
        }

		public string[] Lines => TooltipsBase;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime EditEnd { get; set; }

        [Constructable]
        public ValentineBear(Mobile owner)
            : base(Utility.RandomList(0x48E0, 0x48E2))
        {
            Weight = 1.0;
            LootType = LootType.Blessed;

            m_OwnerName = owner.Name;
            EditEnd = DateTime.MaxValue;
        }

        public void OnFlip(Mobile from)
        {
            if (ItemID == 0x48E0 || ItemID == 0x48E2)
                ItemID = ItemID + 1;
            else
                ItemID = ItemID - 1;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (from is PlayerMobile && EditEnd > DateTime.UtcNow)
                {
                    BaseGump.SendGump(new AddCustomizableMessageGump((PlayerMobile)from, this, 1150294, 1150293));
                }
            }
            else
            {
                from.SendLocalizedMessage(1116249); // That must be in your backpack for you to use it.
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1150295, m_OwnerName); // ~1_NAME~'s St. Valentine Bear
        }

        public ValentineBear(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_OwnerName);
            writer.Write(EditEnd);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_OwnerName = reader.ReadString();
            EditEnd = reader.ReadDateTime();

			if (version < 1)
			{
				int lines = reader.ReadInt();

				for (int i = 0; i < lines && i < Lines.Length; i++)
					Lines[i] = reader.ReadString();
			}
        }
    }
}
