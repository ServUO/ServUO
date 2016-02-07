using System;

namespace Server.Ethics
{
    public abstract class Power
    {
        protected PowerDefinition m_Definition;
        public PowerDefinition Definition
        {
            get
            {
                return this.m_Definition;
            }
        }
        public virtual bool CheckInvoke(Player from)
        {
            if (!from.Mobile.CheckAlive())
                return false;

            if (from.Power < this.m_Definition.Power)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You lack the power to invoke this ability.");
                return false;
            }

            return true;
        }

        public abstract void BeginInvoke(Player from);

        public virtual void FinishInvoke(Player from)
        {
            from.Power -= this.m_Definition.Power;
        }
    }
}