using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.VeteranRewards
{
    public class RewardConfirmGump : Gump
    {
        private readonly Mobile m_From;
        private readonly RewardEntry m_Entry;
        public RewardConfirmGump(Mobile from, RewardEntry entry)
            : base(0, 0)
        {
            this.m_From = from;
            this.m_Entry = entry;

            from.CloseGump(typeof(RewardConfirmGump));

            this.AddPage(0);

            this.AddBackground(10, 10, 500, 300, 2600);

            this.AddHtmlLocalized(30, 55, 300, 35, 1006000, false, false); // You have selected:

            if (entry.NameString != null)
                this.AddHtml(335, 55, 150, 35, entry.NameString, false, false);
            else
                this.AddHtmlLocalized(335, 55, 150, 35, entry.Name, false, false);

            this.AddHtmlLocalized(30, 95, 300, 35, 1006001, false, false); // This will be assigned to this character:
            this.AddLabel(335, 95, 0, from.Name);

            this.AddHtmlLocalized(35, 160, 450, 90, 1006002, true, true); // Are you sure you wish to select this reward for this character?  You will not be able to transfer this reward to another character on another shard.  Click 'ok' below to confirm your selection or 'cancel' to go back to the selection screen.

            this.AddButton(60, 265, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(95, 266, 150, 35, 1006044, false, false); // Ok

            this.AddButton(295, 265, 4017, 4019, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(330, 266, 150, 35, 1006045, false, false); // Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (!RewardSystem.HasAccess(this.m_From, this.m_Entry))
                    return;

                Item item = this.m_Entry.Construct();

                if (item != null)
                {
                    if (item is Server.Items.RedSoulstone)
                        ((Server.Items.RedSoulstone)item).Account = this.m_From.Account.Username;

                    if (item is Server.Items.LighthouseAddonDeed)
                        ((Server.Items.LighthouseAddonDeed)item).Account = this.m_From.Account.Username;

                    if (RewardSystem.ConsumeRewardPoint(this.m_From))
                        this.m_From.AddToBackpack(item);
                    else
                        item.Delete();
                }
            }

            int cur, max;

            RewardSystem.ComputeRewardInfo(this.m_From, out cur, out max);

            if (cur < max)
                this.m_From.SendGump(new RewardNoticeGump(this.m_From));
        }
    }
}