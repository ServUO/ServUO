using Server.Gumps;
using Server.Mobiles;

namespace Server.Services.TownCryer
{
    public class TownCryerEventModeratorGump : BaseTownCryerGump
    {
        public TownCryerModeratorEntry Entry { get; private set; }

        public TownCryerEventModeratorGump(PlayerMobile pm, TownCrier cryer, TownCryerModeratorEntry entry)
            : base(pm, cryer)
        {
            Entry = entry;
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            AddHtml(58, 150, 724, 20, Entry.Title, false, false);
            AddHtmlLocalized(58, 180, 724, 20, 1154760, Entry.ModeratorName, 0, false, false); // By: ~1_NAME~

            AddHtml(58, 215, 724, 205, Entry.Body1, false, false);
            AddHtml(58, 280, 724, 205, Entry.Body2, false, false);
            AddHtml(58, 345, 724, 205, Entry.Body3, false, false);

            AddImage(85, 425, 0x5EF);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                TownCryerGump gump = new TownCryerGump(User, Cryer)
                {
                    Category = TownCryerGump.GumpCategory.EventModerator
                };
                SendGump(gump);
            }
        }
    }
}