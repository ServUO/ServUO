using System;
using Server.Network;
using Server.Misc;
using Server.Items;
using Server.Gumps;

namespace Server.Items
{
    public class NameChangeToken : Item
    {

        [Constructable]
        public NameChangeToken() : base(0x2AAA)
        {
            base.Weight = 5.0;
            base.Name = "a name change token";
        }

        public NameChangeToken(Serial serial)
            : base(serial)
        {
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

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) // Make sure its in their pack 
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it. 
            }
            else
            {
                // Do namechange 
                from.SendGump(new NameChangeGump(from, this));
            }
        }
    }

    public class NameChangeGump : Gump
    {
        private Mobile m_From;
        private Item m_Item;

        public NameChangeGump(Mobile from, Item item)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Item = item;

            AddPage(0);
            AddBackground(0, 0, 250, 100, 2600);
            AddImageTiled(60, 33, 123, 18, 2624);
            AddImageTiled(61, 34, 121, 16, 3504);
            AddLabel(90, 15, 0x384, "New Name:");
            AddTextEntry(62, 32, 121, 15, 0x384, 0, m_From.Name);
            AddButton(80, 60, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);//Ok 
            AddButton(140, 60, 0xFB1, 0xFB3, 2, GumpButtonType.Reply, 0);//Cancel 
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 1) // Chose Name 
            {
                bool isValid = true;

                string newName = (string)info.GetTextEntry(0).Text;

                if (m_Item.Deleted)

                    isValid = false;

                if (!m_Item.IsChildOf(m_From.Backpack)) // Make sure its in their pack 
                {
                    isValid = false;

                    m_From.SendLocalizedMessage(1042001); // That must be in your pack for you to use it. 
                }


                if (!NameVerification.Validate(newName, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote))
                {
                    isValid = false;

                    m_From.SendLocalizedMessage(1005572); // This is not an acceptable name 
                }

                if (isValid)
                {
                    m_From.Name = newName;

                    m_Item.Delete();
                }

            }
        }

    }
}