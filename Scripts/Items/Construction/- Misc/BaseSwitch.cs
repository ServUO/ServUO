using System;
using Server.Network;

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
            this.Movable = false;
            this.m_TurnOn = TurnOn;
            this.m_TurnOff = TurnOff;
            this.m_LocMessageA = LocMessageA;
            this.m_LocMessageB = LocMessageB;
            this.m_Used = false;
            this.m_Working = Working;
        }

        [Constructable]
        public BaseSwitch(int TurnOff, int TurnOn)
            : base(TurnOff)
        {
            this.Movable = false;
            this.m_TurnOn = TurnOn;
            this.m_TurnOff = TurnOff;
            this.m_LocMessageA = 0;
            this.m_LocMessageB = 0;
            this.m_Used = false;
            this.m_Working = false;
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
				
                if (this.m_LocMessageA == 0)
                    MessageA = 500357 + Utility.Random(5);
                else
                    MessageA = this.m_LocMessageA;
				
                int MessageB = 0;
				
                if (this.m_LocMessageB == 0)
                    MessageB = 500357 + Utility.Random(5);
                else
                    MessageB = this.m_LocMessageB;

                /*
                500357 - If this lever ever did anything, it doesn't do it anymore.
                500358 - The lever feels loose, and you realize it no longer controls anything.
                500359 - You flip the lever and think you hear something, but realize it was just your imagination.
                500360 - The lever flips without effort, doing nothing.
                */
					
                if (this.ItemID == this.m_TurnOff && this.m_Used == false)
                {
                    this.ItemID = this.m_TurnOn;
                    this.m_Used = true;
                    Effects.PlaySound(this.Location, this.Map, 0x3E8);
					
                    m.LocalOverheadMessage(MessageType.Regular, 0, MessageA); //Message received when it is turned on by first time.

                    //This call to another method to do something special, so you don't need
                    //to override OnDoubleClick and rewrite this section again.
                    if (this.m_Working == true)
                    {
                        this.DoSomethingSpecial(m);
                    }
					
                    //Refresh time of two minutes, equal to RunUO's RaiseSwith
                    Timer.DelayCall(TimeSpan.FromMinutes(2.0), delegate()
                    {
                        this.ItemID = this.m_TurnOff;
                        this.m_Used = false;
                    });
                }
                else if (this.ItemID == this.m_TurnOff && this.m_Used == true)
                {
                    this.ItemID = this.m_TurnOn;
                    Effects.PlaySound(this.Location, this.Map, 0x3E8);
                    m.LocalOverheadMessage(MessageType.Regular, 0, MessageB); //Message received after click it again until the refresh.
                }
                else //TurnOn and m_Used true
                {
                    this.ItemID = this.m_TurnOff;
                    Effects.PlaySound(this.Location, this.Map, 0x3E8);
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
            writer.Write((int)0); // version
            writer.Write(this.m_TurnOn);
            writer.Write(this.m_TurnOff);
            writer.Write(this.m_LocMessageA);
            writer.Write(this.m_LocMessageB);
            writer.Write(this.m_Working);
            writer.Write(this.m_Used);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            this.m_TurnOn = reader.ReadInt();
            this.m_TurnOff = reader.ReadInt();
            this.m_LocMessageA = reader.ReadInt();
            this.m_LocMessageB = reader.ReadInt();
            this.m_Working = reader.ReadBool();
            this.m_Used = reader.ReadBool();
            this.m_Used = false;
            this.ItemID = this.m_TurnOff;
        }
    }
}