using Server.Network;
using System;

namespace Server.Items
{
    public class BaseSwitch : Item
    {
        private int m_TurnOn;
        private int m_TurnOff;
        private int m_LocMessageA;
        private int m_LocMessageB;
        private bool m_Used;
        private bool m_Working;
        [Constructable]
        public BaseSwitch(int TurnOff, int TurnOn, int LocMessageA, int LocMessageB, bool Working)
            : base(TurnOff)
        {
            Movable = false;
            m_TurnOn = TurnOn;
            m_TurnOff = TurnOff;
            m_LocMessageA = LocMessageA;
            m_LocMessageB = LocMessageB;
            m_Used = false;
            m_Working = Working;
        }

        [Constructable]
        public BaseSwitch(int TurnOff, int TurnOn)
            : base(TurnOff)
        {
            Movable = false;
            m_TurnOn = TurnOn;
            m_TurnOff = TurnOff;
            m_LocMessageA = 0;
            m_LocMessageB = 0;
            m_Used = false;
            m_Working = false;
        }

        public BaseSwitch(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(this, 2))
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }
            else
            {
                int MessageA = 0;

                if (m_LocMessageA == 0)
                    MessageA = 500357 + Utility.Random(5);
                else
                    MessageA = m_LocMessageA;

                int MessageB = 0;

                if (m_LocMessageB == 0)
                    MessageB = 500357 + Utility.Random(5);
                else
                    MessageB = m_LocMessageB;

                /*
                500357 - If this lever ever did anything, it doesn't do it anymore.
                500358 - The lever feels loose, and you realize it no longer controls anything.
                500359 - You flip the lever and think you hear something, but realize it was just your imagination.
                500360 - The lever flips without effort, doing nothing.
                */

                if (ItemID == m_TurnOff && m_Used == false)
                {
                    ItemID = m_TurnOn;
                    m_Used = true;
                    Effects.PlaySound(Location, Map, 0x3E8);

                    m.LocalOverheadMessage(MessageType.Regular, 0, MessageA); //Message received when it is turned on by first time.

                    //This call to another method to do something special, so you don't need
                    //to override OnDoubleClick and rewrite this section again.
                    if (m_Working == true)
                    {
                        DoSomethingSpecial(m);
                    }

                    //Refresh time of two minutes, equal to RunUO's RaiseSwith
                    Timer.DelayCall(TimeSpan.FromMinutes(2.0), delegate ()
                    {
                        ItemID = m_TurnOff;
                        m_Used = false;
                    });
                }
                else if (ItemID == m_TurnOff && m_Used == true)
                {
                    ItemID = m_TurnOn;
                    Effects.PlaySound(Location, Map, 0x3E8);
                    m.LocalOverheadMessage(MessageType.Regular, 0, MessageB); //Message received after click it again until the refresh.
                }
                else //TurnOn and m_Used true
                {
                    ItemID = m_TurnOff;
                    Effects.PlaySound(Location, Map, 0x3E8);
                    m.LocalOverheadMessage(MessageType.Regular, 0, MessageB); //Message received after click it again until the refresh.
                }
            }
        }

        public virtual void DoSomethingSpecial(Mobile from)
        {
            from.LocalOverheadMessage(MessageType.Regular, 0, 1116629); //It does Nothing!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write(m_TurnOn);
            writer.Write(m_TurnOff);
            writer.Write(m_LocMessageA);
            writer.Write(m_LocMessageB);
            writer.Write(m_Working);
            writer.Write(m_Used);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_TurnOn = reader.ReadInt();
            m_TurnOff = reader.ReadInt();
            m_LocMessageA = reader.ReadInt();
            m_LocMessageB = reader.ReadInt();
            m_Working = reader.ReadBool();
            m_Used = reader.ReadBool();
            m_Used = false;
            ItemID = m_TurnOff;
        }
    }
}