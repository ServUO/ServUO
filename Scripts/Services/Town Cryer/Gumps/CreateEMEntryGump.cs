using Server;
using System;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Services.TownCryer
{
    public class CreateEMEntryGump : BaseTownCryerGump
    {
        public TownCryerModeratorEntry Entry { get; set; }
        public bool Edit { get; private set; }

        public CreateEMEntryGump(PlayerMobile pm, TownCrier cryer, TownCryerModeratorEntry entry = null)
            : base(pm, cryer)
        {
            Entry = entry;

            if (Entry != null)
            {
                Edit = true;
            }
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            AddHtmlLocalized(58, 150, 100, 20, 1158027, false, false); // Author:
            AddLabel(105, 150, 0, String.Format("EM {0}", User.Name));

            AddHtmlLocalized(58, 180, 100, 20, 1158026, false, false); // Headline:
            AddBackground(58, 200, 740, 20, 0x2486);
            AddTextEntry(59, 200, 739, 20, 0, 1, Entry != null ? Entry.Title : "");

            AddHtmlLocalized(58, 220, 120, 20, 1158028, false, false); // Body Paragraph 1:
            AddBackground(58, 240, 740, 40, 0x2486);
            AddTextEntry(59, 240, 739, 40, 0, 2, Entry != null ? Entry.Body1 : "");

            AddHtmlLocalized(58, 280, 120, 20, 1158029, false, false); // Body Paragraph 2:
            AddBackground(58, 300, 740, 40, 0x2486);
            AddTextEntry(59, 300, 739, 40, 0, 3, Entry != null ? Entry.Body2 : "");

            AddHtmlLocalized(58, 340, 120, 20, 1158030, false, false); // Body Paragraph 3:
            AddBackground(58, 360, 740, 40, 0x2486);
            AddTextEntry(59, 360, 739, 40, 0, 4, Entry != null ? Entry.Body3 : "");

            AddBackground(155, 405, 20, 20, 0x2486);
            AddHtmlLocalized(58, 405, 100, 20, 1158031, false, false); // Expiry (in days):
            AddTextEntry(156, 405, 19, 20, 0, 5, "");

            AddImage(85, 425, 0x5EF);

            AddButton(40, 615, 0x601, 0x602, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(63, 615, 150, 20, 1077787, false, false); // Submit
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                string headline = info.GetTextEntry(1).Text;
                string body1 = info.GetTextEntry(2).Text;
                string body2 = info.GetTextEntry(3).Text;
                string body3 = info.GetTextEntry(4).Text;
                string exp = info.GetTextEntry(5).Text;

                int expires = Utility.ToInt32(exp);

                if (Entry == null)
                {
                    Entry = new TownCryerModeratorEntry(User, expires, headline, body1, body2, body3);
                }
                else
                {
                    Entry.Title = headline;
                    Entry.Body1 = body1;
                    Entry.Body2 = body2;
                    Entry.Body3 = body3;

                    if (expires >= 1 && expires <= 30)
                    {
                        Entry.Expires = DateTime.Now + TimeSpan.FromDays(expires);
                    }
                }

                if(expires < 1 || expires > 30)
                {
                    User.SendLocalizedMessage(1158033); // The expiry can be between 1 and 30 days. Please check your entry and try again.
                }
                else if (string.IsNullOrEmpty(headline) || string.IsNullOrEmpty(body1) || headline.Length < 5 || body1.Length < 5)
                {
                    User.SendLocalizedMessage(1158032); // You have made an illegal entry.  Check your entries and try again.
                }
                else
                {
                    if (!Edit)
                    {
                        TownCryerSystem.AddEntry(Entry);
                    }

                    User.SendLocalizedMessage(1158039); // Your entry has been submitted.

                    BaseGump.SendGump(new TownCryerGump(User, Cryer, 0, TownCryerGump.GumpCategory.EventModerator));
                    return;
                }

                Refresh();
            }
        }
    }
}