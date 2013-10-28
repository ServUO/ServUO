using System;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class LevelUpAcceptGump : Gump
    {
        private readonly LevelUpScroll m_Scroll;
        private readonly Mobile m_From;
        public LevelUpAcceptGump(LevelUpScroll scroll, Mobile from)
            : base(0, 0)
        {
            this.m_Scroll = scroll;
            this.m_From = from;

            string PaymentMsg = null;
            if (LevelItems.RewardBlacksmith && LevelItems.BlacksmithRewardAmt > 0)
                PaymentMsg = "<BR><BR>If you accept and the process is successful, you will be given additional compensation of " + LevelItems.BlacksmithRewardAmt + ".";

            this.Closable = false;
            this.Disposable = false;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);

            this.AddBackground(25, 22, 318, 268, 9390);
            this.AddLabel(52, 27, 0, @"Level Increase Request");
            this.AddLabel(52, 60, 0, @"Requested By:");
            this.AddLabel(52, 81, 0, @"Level Amount:");
            this.AddHtml(49, 109, 271, 116, @"<CENTER><U>Max Level Increase Request</U><BR><BR>Someone has requested your expert services in increasing the max levels of a levelable item." + PaymentMsg + "<BR><BR>Do you accept their offer?", (bool)false, (bool)true);
            this.AddButton(50, 235, 4023, 4024, 1, GumpButtonType.Reply, 0);
            this.AddButton(83, 235, 4017, 4018, 2, GumpButtonType.Reply, 0);
            if (this.m_From != null)
                this.AddLabel(155, 60, 390, this.m_From.Name.ToString());
            if (this.m_Scroll != null)
                this.AddLabel(155, 81, 390, this.m_Scroll.Value.ToString());
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile smith = state.Mobile;

            if (smith == null)
                return;

            //Accept
            if (info.ButtonID == 1)
            {
                if (this.m_From != null && this.m_Scroll != null)
                {
                    if (this.m_From != null)
                    {
                        this.m_Scroll.BlacksmithValidated = true;
                        this.m_From.CloseGump(typeof(AwaitingSmithApprovalGump));
                        this.m_From.SendMessage("They have validated your scroll.  Select a levelable item to increase max levels or ESC to apply at another time.");
                        this.m_From.Target = new LevelUpScroll.LevelItemTarget(this.m_Scroll); // Call our target
                    }

                    if (smith != null) //Accepted... send message to smith and pay them bonus reward
                    {
                        smith.SendMessage("Thank you for your services!");
                        if (smith != this.m_From && LevelItems.RewardBlacksmith && LevelItems.BlacksmithRewardAmt > 0)
                        {
                            smith.AddToBackpack(new BankCheck(LevelItems.BlacksmithRewardAmt));
                            smith.SendMessage("A Bonus payment has been added to your pack.");
                        }
                    }
                }
                else
                {
                    if (this.m_From != null && smith != null)
                    {
                        this.m_From.SendMessage("There was a problem validating this scroll.");
                        smith.SendMessage("There was a problem validating this scroll.");
                    }
                }
            }

            //Decline
            if (info.ButtonID == 2)
            {
                smith.SendMessage("You have declined their offer.");

                if (this.m_From != null)
                    this.m_From.CloseGump(typeof(AwaitingSmithApprovalGump));
                this.m_From.SendMessage("They have declined your offer");
            }
        }
    }
}