/**************************************
*Script Name: Staff Runebook          *
*Author: Joeku                        *
*For use with RunUO 2.0 RC2           *
*Client Tested with: 6.0.9.2          *
*Version: 1.10                        *
*Initial Release: 11/25/07            *
*Revision Date: 02/04/09              *
**************************************/

using System;
using Server;
using Server.Items;

namespace Joeku.SR
{
    public class SR_RuneGate : Moongate
    {
        public SR_RuneGate(Point3D target, Map map)
            : base(target, map)
        {
            this.Map = map;

            if (this.ShowFeluccaWarning && map == Map.Felucca)
                this.ItemID = 0xDDA;

            this.Dispellable = false;

            InternalTimer t = new InternalTimer(this);
            t.Start();
        }

        public SR_RuneGate(Serial serial)
            : base(serial)
        {
        }

        public override bool ShowFeluccaWarning
        {
            get
            {
                return false/*Core.AOS*/;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            this.Delete();
        }

        private class InternalTimer : Timer
        {
            private readonly Item m_Item;
            public InternalTimer(Item item)
                : base(TimeSpan.FromSeconds(30.0))
            {
                this.Priority = TimerPriority.OneSecond;
                this.m_Item = item;
            }

            protected override void OnTick()
            {
                this.m_Item.Delete();
            }
        }
    }
}