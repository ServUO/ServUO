using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
    public class HorseBreederGump : FactionGump
    {
        private readonly PlayerMobile m_From;
        private readonly Faction m_Faction;
        public HorseBreederGump(PlayerMobile from, Faction faction)
            : base(20, 30)
        {
            this.m_From = from;
            this.m_Faction = faction;

            this.AddPage(0);

            this.AddBackground(0, 0, 320, 280, 5054);
            this.AddBackground(10, 10, 300, 260, 3000);

            this.AddHtmlText(20, 30, 300, 25, faction.Definition.Header, false, false);

            this.AddHtmlLocalized(20, 60, 300, 25, 1018306, false, false); // Purchase a Faction War Horse
            this.AddItem(70, 120, 0x3FFE);

            this.AddItem(150, 120, 0xEF2);
            this.AddLabel(190, 122, 0x3E3, FactionWarHorse.SilverPrice.ToString("N0")); // NOTE: Added 'N0'

            this.AddItem(150, 150, 0xEEF);
            this.AddLabel(190, 152, 0x3E3, FactionWarHorse.GoldPrice.ToString("N0")); // NOTE: Added 'N0'

            this.AddHtmlLocalized(55, 210, 200, 25, 1011011, false, false); // CONTINUE
            this.AddButton(20, 210, 4005, 4007, 1, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(55, 240, 200, 25, 1011012, false, false); // CANCEL
            this.AddButton(20, 240, 4005, 4007, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID != 1)
                return;

            if (Faction.Find(this.m_From) != this.m_Faction)
                return;

            Container pack = this.m_From.Backpack;

            if (pack == null)
                return;

            FactionWarHorse horse = new FactionWarHorse(this.m_Faction);

            if ((this.m_From.Followers + horse.ControlSlots) > this.m_From.FollowersMax)
            {
                // TODO: Message?
                horse.Delete();
            }
            else
            {
                if (pack.GetAmount(typeof(Silver)) < FactionWarHorse.SilverPrice)
                {
                    sender.Mobile.SendLocalizedMessage(1042204); // You do not have enough silver.
                    horse.Delete();
                }
                else if (pack.GetAmount(typeof(Gold)) < FactionWarHorse.GoldPrice)
                {
                    sender.Mobile.SendLocalizedMessage(1042205); // You do not have enough gold.
                    horse.Delete();
                }
                else if (pack.ConsumeTotal(typeof(Silver), FactionWarHorse.SilverPrice) && pack.ConsumeTotal(typeof(Gold), FactionWarHorse.GoldPrice))
                {
                    horse.Controlled = true;
                    horse.ControlMaster = this.m_From;

                    horse.ControlOrder = OrderType.Follow;
                    horse.ControlTarget = this.m_From;

                    horse.MoveToWorld(this.m_From.Location, this.m_From.Map);
                }
                else
                {
                    horse.Delete();
                }
            }
        }
    }
}