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
            m_From = from;
            m_Entry = entry;

            from.CloseGump(typeof(RewardConfirmGump));

            AddPage(0);

            AddBackground(10, 10, 500, 300, 2600);

            AddHtmlLocalized(30, 55, 300, 35, 1006000, false, false); // You have selected:

            if (entry.NameString != null)
                AddHtml(335, 55, 150, 35, entry.NameString, false, false);
            else
                AddHtmlLocalized(335, 55, 150, 35, entry.Name, false, false);

            AddHtmlLocalized(30, 95, 300, 35, 1006001, false, false); // This will be assigned to this character:
            AddLabel(335, 95, 0, from.Name);

            AddHtmlLocalized(35, 160, 450, 90, 1006002, true, true); // Are you sure you wish to select this reward for this character?  You will not be able to transfer this reward to another character on another shard.  Click 'ok' below to confirm your selection or 'cancel' to go back to the selection screen.

            AddButton(60, 265, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(95, 266, 150, 35, 1006044, false, false); // Ok

            AddButton(295, 265, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(330, 266, 150, 35, 1006045, false, false); // Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (!RewardSystem.HasAccess(m_From, m_Entry))
                    return;

                Item item = m_Entry.Construct();

                if (item != null)
                {
                    if (item is Items.RedSoulstone)
                        ((Items.RedSoulstone)item).Account = m_From.Account.Username;

                    if (item is Items.LighthouseAddonDeed)
                        ((Items.LighthouseAddonDeed)item).Account = m_From.Account.Username;

                    if (RewardSystem.ConsumeRewardPoint(m_From))
                    {
                        #region TOL
                        if (item is Auction.AuctionSafeDeed)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Item it = m_Entry.Construct();
                                m_From.AddToBackpack(it);
                            }
                        }
                        #endregion

                        m_From.AddToBackpack(item);
                    }
                    else
                        item.Delete();
                }
            }

            int cur, max;

            RewardSystem.ComputeRewardInfo(m_From, out cur, out max);

            if (cur < max)
                m_From.SendGump(new RewardNoticeGump(m_From));
        }
    }
}