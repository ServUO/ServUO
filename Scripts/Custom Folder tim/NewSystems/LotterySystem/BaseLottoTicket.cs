using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;

/* To add additional ticket types, update the following scripts:
 * 
 * This:                   TicketType enum
 * ScratcherGump.cs:       TicketType enum switches in the contructor and at in GetBackground() method.
 * ScratcherStongGump.cs:  The appropriate info in ticket info region, new page for its stats, and a new response to buy a ticket. 
 *                         Background length will need to be adjusted accordingly as well.
 * ScratcherLotto:         GetGameType() Method will need to reflect the ticket name.
 * LottoTicketVendor:      Add the new ticket, ID, Hue to the SBInfo
 */

namespace Server.Engines.LotterySystem
{
    public enum TicketType
    {           
        GoldenTicket = 1,         //1 mil jackpot
        CrazedCrafting = 2,       //250k - bonus if owner is 100+ craft skill;
        SkiesTheLimit = 3,        //Progressive
        Powerball = 4,            //Standard Powerball 5white/1red
    }

    public class BaseLottoTicket : Item, IOwnerRestricted
    {
        //Serialized fields/props
        private int m_Payout;
        private bool m_CashedOut;
        private bool m_Checked;
        private Mobile m_Owner;
        private TicketType m_Type;

        private int m_Scratch1;
        private int m_Scratch2;
        private int m_Scratch3;
        private bool m_FreeTicket;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Payout { get { return m_Payout; } set { m_Payout = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool CashedOut { get { return m_CashedOut; } set { m_CashedOut = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Checked { get { return m_Checked; } set { m_Checked = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public TicketType Type { get { return m_Type; } set { m_Type = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Scratch1 { get { return m_Scratch1; } set { m_Scratch1 = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Scratch2 { get { return m_Scratch2; } set { m_Scratch2 = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Scratch3 { get { return m_Scratch3; } set { m_Scratch3 = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool FreeTicket { get { return m_FreeTicket; } set { m_FreeTicket = value; } }

        public virtual string OwnerName { get { return m_Owner != null ? m_Owner.Name : "unknown"; } set { } }

        public BaseLottoTicket(Mobile owner, TicketType type, bool quickScratch) : base(0x0FF3)
        {
            m_Type = type;
            m_Owner = owner;
            m_CashedOut = false;
            m_Payout = 0;
            m_Checked = false;
            m_FreeTicket = false;

            if (quickScratch)
                DoQuickScratch();
        }

        public virtual void CheckScratches()
        {
        }

        public virtual bool DoScratch(int scratch, Mobile from)
        {
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Owner == null)
                m_Owner = from;

            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001);

            else if (from != m_Owner)
                from.SendMessage("Only the owner can view this ticket.");

            else if (this is PowerBallTicket)
            {
                if (from.HasGump(typeof(PowerballTicketGump)))
                    from.CloseGump(typeof(PowerballTicketGump));

                from.SendGump(new PowerballTicketGump((PowerBallTicket)this, from));
            }
            else
            {
                if (from.HasGump(typeof(ScratcherGump)))
                    from.CloseGump(typeof(ScratcherGump));

                from.SendGump(new ScratcherGump(this, from));
            }

            InvalidateProperties();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            string amount = "0";
            if (m_Payout > 0)
            {
                amount = String.Format("{0:##,###,###}", m_Payout);
                list.Add(1060847, "{0}\t{1}", "Winnings:", amount);
            }
            if (m_FreeTicket)
                list.Add("Free Ticket");
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            base.OnItemLifted(from, item);

            if (this is PowerBallTicket)
            {
                from.CloseGump(typeof(PowerballTicketGump));
            }
            else
            {
                from.CloseGump(typeof(ScratcherGump));
            }
        }

        private void DoQuickScratch()
        {
            if (m_Owner == null)
                return;

            for (int i = 1; i < 4; ++i)
                DoScratch(i, m_Owner);

            m_Owner.SendMessage("You quickly scratch your ticket once you purchase it.");

            m_Owner.PlaySound(0x249);

            if (m_Owner.HasGump(typeof(ScratcherGump)))
                m_Owner.CloseGump(typeof(ScratcherGump));

            m_Owner.SendGump(new ScratcherGump(this, m_Owner));

            if (ScratcherLotto.Stone != null && ScratcherLotto.Stone.DeleteTicketOnLoss && m_Checked && !m_FreeTicket && m_Payout == 0)
                Timer.DelayCall(TimeSpan.FromSeconds(0.5), new TimerCallback(TrashLosingTicket));
            
        }

        public void TrashLosingTicket()
        {
            if (m_Owner != null)
            {
                m_Owner.SendMessage("You trash the losing ticket.");
                m_Owner.SendSound(0x48, m_Owner.Location);
            }

            Delete();
        }

        public BaseLottoTicket(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //Version

            writer.Write(m_Owner);
            writer.Write(m_Payout);
            writer.Write(m_CashedOut);
            writer.Write(m_Checked);
            writer.Write((int)m_Type);

            writer.Write(m_Scratch1);
            writer.Write(m_Scratch2);
            writer.Write(m_Scratch3);
            writer.Write(m_FreeTicket);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Owner = reader.ReadMobile();
            m_Payout = reader.ReadInt();
            m_CashedOut = reader.ReadBool();
            m_Checked = reader.ReadBool();
            m_Type = (TicketType)reader.ReadInt();

            m_Scratch1 = reader.ReadInt();
            m_Scratch2 = reader.ReadInt();
            m_Scratch3 = reader.ReadInt();
            m_FreeTicket = reader.ReadBool();
        }
    }
}