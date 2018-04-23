using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Engines.LotterySystem;
using Server.Gumps;
using Server.Items;

namespace Server.Items
{
    public class PowerBallTicket : BaseLottoTicket
    {
        private PowerBall m_PowerBall;
        private int m_Game;
        private List<TicketEntry> m_Entries = new List<TicketEntry>();

        [CommandProperty(AccessLevel.GameMaster)]
        public int GameNumber { get { return m_Game; } }

        public List<TicketEntry> Entries { get { return m_Entries; } }

        [Constructable]
        public PowerBallTicket() : base(null, TicketType.Powerball, false)
        {
            Name = "a powerball ticket";
            LootType = LootType.Blessed;
            Hue = 2106;

            if (PowerBall.Instance != null && PowerBall.Game != null)
            {
                m_PowerBall = PowerBall.Game;
                PowerBall.Instance.AddTicket(this);
                m_Game = m_PowerBall.GameNumber;
            }
            else
                m_Game = 0;
        }

        [Constructable]
        public PowerBallTicket(Mobile owner, PowerBall powerball) : base (owner, TicketType.Powerball, false)
        {
            Name = "a powerball ticket";
            LootType = LootType.Blessed;
            Hue = 2106;

            m_PowerBall = powerball;

            if (m_PowerBall != null)
                m_Game = m_PowerBall.GameNumber;
            else
                m_Game = 0;

            if (PowerBall.Instance != null)
                PowerBall.Instance.AddTicket(this);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Payout > 0 || m_PowerBall == null || m_Game != m_PowerBall.GameNumber )
                list.Add(1150487); //[Expired]
            else
                list.Add(3005117); //[Active]
        }

        /*public override void OnDoubleClick(Mobile from)
        {
            if (Owner == null)
                Owner = from;

            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001);

            else if (from != Owner)
                from.SendMessage("Only the owner can view this ticket.");

            else
            {
                if (from.HasGump(typeof(TicketGump)))
                    from.CloseGump(typeof(TicketGump));

                from.SendGump(new TicketGump(this, from));
            }

            base.OnDoubleClick(from);               
        }*/

        public void AddEntry(TicketEntry entry)
        {
            if (!m_Entries.Contains(entry))  
                m_Entries.Add(entry);
        }

        public override void OnAfterDelete()
        {
            if (PowerBall.Instance != null)
                PowerBall.Instance.RemoveTicket(this);

            if (m_Entries.Count > 0)
                m_Entries.Clear();
        }

        public PowerBallTicket(Serial serial) : base( serial )
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
 
            writer.Write((int) 0); //Version

            writer.Write(m_Game);
            writer.Write(m_Entries.Count);

            for (int i = 0; i < m_Entries.Count; ++i)
            {
                TicketEntry entry = m_Entries[i];

                writer.Write(entry.One);
                writer.Write(entry.Two);
                writer.Write(entry.Three);
                writer.Write(entry.Four);
                writer.Write(entry.Five);
                writer.Write(entry.PowerBall);
                writer.Write(entry.Winner);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Game = reader.ReadInt();
            int count = reader.ReadInt();

            for (int i = 0; i < count; ++i)
            {
                int one = reader.ReadInt();
                int two = reader.ReadInt();
                int three = reader.ReadInt();
                int four = reader.ReadInt();
                int five = reader.ReadInt();
                int six = reader.ReadInt();
                bool winner = reader.ReadBool();
                
                new TicketEntry(this, one, two, three, four, five, six, winner);
            }

            m_PowerBall = PowerBall.Game;

            if (PowerBall.Instance != null && m_PowerBall != null && m_Game == m_PowerBall.GameNumber)
                PowerBall.Instance.AddTicket(this);
        }
    }

    public class TicketEntry
    {
        private PowerBallTicket m_Ticket;
        private int m_One;
        private int m_Two;
        private int m_Three;
        private int m_Four;
        private int m_Five;
        private int m_PowerBall;
        private bool m_Winner;

        public PowerBallTicket Ticket { get { return m_Ticket; } }
        public int One { get { return m_One; } }
        public int Two { get { return m_Two; } }
        public int Three { get { return m_Three; } }
        public int Four { get { return m_Four; } }
        public int Five{ get { return m_Five; } }
        public int PowerBall { get { return m_PowerBall; } }
        public bool Winner { get { return m_Winner; } set { m_Winner = value; } }

        public TicketEntry (PowerBallTicket ticket, int one, int two, int three, int four, int five, bool winner) : this (ticket, one, two, three, four, five, 0, winner)
        {
        }

        public TicketEntry(PowerBallTicket ticket, int one, int two, int three, int four, int five, int six, bool winner)
        {
            m_Ticket = ticket;

            m_One = one;
            m_Two = two;
            m_Three = three;
            m_Four = four;
            m_Five = five;
            m_PowerBall = six;
            m_Winner = winner;

            m_Ticket.AddEntry(this);
        } 
    }
}