using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class NewPlayerTicket : Item
    {
        private Mobile m_Owner;
        [Constructable]
        public NewPlayerTicket()
            : base(0x14EF)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public NewPlayerTicket(Serial serial)
            : base(serial)
        {
        }

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
            }
        }
        public override int LabelNumber => 1062094;// a young player ticket
        public override bool DisplayLootType => true;
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1041492); // This is half a prize ticket! Double-click this ticket and target any other ticket marked NEW PLAYER and get a prize! This ticket will only work for YOU, so don't give it away!
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

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Owner = reader.ReadMobile();
                        break;
                    }
            }

            if (Name == "a young player ticket")
                Name = null;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from != m_Owner)
            {
                from.SendLocalizedMessage(501926); // This isn't your ticket! Shame on you! You have to use YOUR ticket.
            }
            else if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.SendLocalizedMessage(501927); // Target any other ticket marked NEW PLAYER to win a prize.
                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            private readonly NewPlayerTicket m_Ticket;
            public InternalTarget(NewPlayerTicket ticket)
                : base(2, false, TargetFlags.None)
            {
                m_Ticket = ticket;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted == m_Ticket)
                {
                    from.SendLocalizedMessage(501928); // You can't target the same ticket!
                }
                else if (targeted is NewPlayerTicket)
                {
                    NewPlayerTicket theirTicket = targeted as NewPlayerTicket;
                    Mobile them = theirTicket.m_Owner;

                    if (them == null || them.Deleted)
                    {
                        from.SendLocalizedMessage(501930); // That is not a valid ticket.
                    }
                    else
                    {
                        from.SendGump(new InternalGump(from, m_Ticket));
                        them.SendGump(new InternalGump(them, theirTicket));
                    }
                }
                else if (targeted is Item && ((Item)targeted).ItemID == 0x14F0)
                {
                    from.SendLocalizedMessage(501931); // You need to find another ticket marked NEW PLAYER.
                }
                else
                {
                    from.SendLocalizedMessage(501929); // You will need to select a ticket.
                }
            }
        }

        private class InternalGump : Gump
        {
            private readonly Mobile m_From;
            private readonly NewPlayerTicket m_Ticket;
            public InternalGump(Mobile from, NewPlayerTicket ticket)
                : base(50, 50)
            {
                m_From = from;
                m_Ticket = ticket;

                AddBackground(0, 0, 400, 385, 0xA28);

                AddHtmlLocalized(30, 45, 340, 70, 1013011, true, true); // Choose the gift you prefer. WARNING: if you cancel, and your partner does not, you will need to find another matching ticket!

                AddButton(46, 128, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 130, 320, 35, 1013012, false, false); // A sextant

                AddButton(46, 163, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 165, 320, 35, 1013013, false, false); // A coupon for a single hair restyling

                AddButton(46, 198, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 200, 320, 35, 1013014, false, false); // A spellbook with all 1st - 4th spells.

                AddButton(46, 233, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 235, 320, 35, 1013015, false, false); // A wand of fireworks

                AddButton(46, 268, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 270, 320, 35, 1013016, false, false); // A spyglass

                AddButton(46, 303, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 305, 320, 35, 1013017, false, false); // Dyes and a dye tub

                AddButton(120, 340, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(154, 342, 100, 35, 1011012, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Ticket.Deleted)
                    return;

                int number = 0;

                Item item = null;
                Item item2 = null;

                switch (info.ButtonID)
                {
                    case 1:
                        item = new Sextant();
                        number = 1010494;
                        break; // A sextant has been placed in your backpack.
                    case 2:
                        item = new HairRestylingDeed();
                        number = 501933;
                        break; // A coupon for a free hair restyling has been placed in your backpack.
                    case 3:
                        item = new Spellbook(0xFFFFFFFF);
                        number = 1010495;
                        break; // A spellbook with all 1st to 4th circle spells has been placed in your backpack.
                    case 4:
                        item = new FireworksWand();
                        number = 501935;
                        break; // A wand of fireworks has been placed in your backpack.
                    case 5:
                        item = new Spyglass();
                        number = 501936;
                        break; // A spyglass has been placed in your backpack.
                    case 6:
                        item = new DyeTub();
                        item2 = new Dyes();
                        number = 501937;
                        break; // The dyes and dye tub have been placed in your backpack.
                }

                if (item != null)
                {
                    m_Ticket.Delete();

                    m_From.SendLocalizedMessage(number);
                    m_From.AddToBackpack(item);

                    if (item2 != null)
                        m_From.AddToBackpack(item2);
                }
            }
        }
    }
}
