using System;

namespace Server.ContextMenus
{
    public class OpenBankEntry : ContextMenuEntry
    {
        private readonly Mobile m_Banker;
        public OpenBankEntry(Mobile from, Mobile banker)
            : base(6105, 12)
        {
            this.m_Banker = banker;
        }

        public override void OnClick()
        {
            if (!this.Owner.From.CheckAlive())
                return;

            if (this.Owner.From.Criminal)
            {
                this.m_Banker.Say(500378); // Thou art a criminal and cannot access thy bank box.
            }
            else
            {
                this.Owner.From.BankBox.Open();
            }
        }
    }
}