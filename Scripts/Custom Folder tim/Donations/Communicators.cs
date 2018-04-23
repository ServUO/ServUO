/************ Communicators.cs  v.0 *****************************************
 *
 *            (C) 2010, Lokai
 * 
 * Description: These are communication devices that are designed to be 
 *     carried by two people. If both are carried, and one is able to 
 *     transmit to the other, they will transmit the speech of the carrier
 *     of the item to the owner of the matching communicator.
 * Usage: They can be given as gifts or sold in shops, but the Bag should 
 *     be given, not the individual communicators. The bag will produce 
 *     matching communicators when opened for the first time. If a 
 *     GameMaster chooses to make the bag reusable, then it will produce 
 *     new matching communicators each time it is opened when empty. The 
 *     communicators are then double-clicked to toggle between send-only, 
 *     receive-only, two-way, or off. Speech is automatically transmitted 
 *     to the person carrying the matching communicator, if the sending
 *     communicator can send, and the target communicator can receive.
 *
 *************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class Communicator : Item
    {
        private enum OnStatus { Off, ReceiveOnly, SendOnly, TwoWay }

        private OnStatus m_On;

        public bool On { get { return !(m_On == OnStatus.Off); } }
        public bool CanReceive { get { return (m_On == OnStatus.ReceiveOnly || m_On == OnStatus.TwoWay); } }
        public bool CanSend { get { return (m_On == OnStatus.SendOnly || m_On == OnStatus.TwoWay); } }

        private Communicator m_Mate;
        public Communicator Mate { get { return m_Mate; } set { m_Mate = value; } }

        [Constructable]
        public Communicator()
            : base(0xE2E)
        {
            Weight = 4.0;
            m_On = OnStatus.Off;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (m_Mate.CanReceive && this.CanSend)
            {
                if (this.RootParent is PlayerMobile)
                {
                    PlayerMobile owner = this.RootParent as PlayerMobile;
                    if (e.Mobile == owner && m_Mate.RootParent is PlayerMobile)
                    {
                        PlayerMobile mate = m_Mate.RootParent as PlayerMobile;
                        mate.SendMessage("FROM - {0}: {1}", e.Mobile.Name, e.Speech);
                    }
                }
            }
            base.OnSpeech(e);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            string status = "";
            switch (m_On)
            {
                case OnStatus.ReceiveOnly: status = "in Receive Only mode"; break;
                case OnStatus.SendOnly: status = "in Send Only mode"; break;
                case OnStatus.TwoWay: status = "in Two-Way mode"; break;
                case OnStatus.Off: status = "Off"; break;
            }
            list.Add("Communication Device [status: {0}]", status);
        }

        public override void OnDoubleClick(Mobile from)
        {
            string status = "";
            switch (m_On)
            {
                case OnStatus.Off: m_On = OnStatus.ReceiveOnly; status = "in Receive Only mode"; break;
                case OnStatus.ReceiveOnly: m_On = OnStatus.SendOnly; status = "in Send Only mode"; break;
                case OnStatus.SendOnly: m_On = OnStatus.TwoWay; status = "in Two-Way mode"; break;
                case OnStatus.TwoWay: m_On = OnStatus.Off; status = "Off"; break;
            }
            from.SendMessage("Communicator is now {0}.", status);
            InvalidateProperties();
        }

        public Communicator(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
            writer.Write(m_Mate);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
            m_Mate = reader.ReadItem() as Communicator;

            m_On = OnStatus.Off;
        }
    }

    public class CommunicatorBag : Bag
    {
        public override string DefaultName
        {
            get { return "a Bag of Personal Communicators"; }
        }

        private bool m_Reusable;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Reusable { get { return m_Reusable; } set { m_Reusable = value; } }

        private bool m_Used;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Used { get { return m_Used; } set { m_Used = value; } }

        [CommandProperty(AccessLevel.Player)]
        public bool IsUsed { get { return m_Used; } }

        [Constructable]
        public CommunicatorBag()
            : base()
        {
            Movable = true;
            Hue = Utility.RandomRedHue();
            m_Used = false;
            m_Reusable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Items.Count == 0 && (m_Reusable || !m_Used))
            {
                Communicator commA = new Communicator();
                Communicator commB = new Communicator();
                commA.Mate = commB;
                commB.Mate = commA;
                DropItem(commA);
                DropItem(commB);
                m_Used = true;
            }
            base.OnDoubleClick(from);
        }

        public CommunicatorBag(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((bool)m_Used);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Used = reader.ReadBool();
        }
    }
}
