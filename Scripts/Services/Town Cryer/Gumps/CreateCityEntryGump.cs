using Server;
using System;
using Server.Mobiles;
using Server.Engines.CityLoyalty;
using Server.Gumps;

namespace Server.Services.TownCryer
{
    public class CreateCityEntryGump : BaseTownCryerGump
    {
        public TownCryerCityEntry Entry { get; set; }
        public bool Edit { get; private set; }
        public City City { get; private set; }

        public CreateCityEntryGump(PlayerMobile pm, TownCrier cryer, City city, TownCryerCityEntry entry = null)
            : base(pm, cryer)
        {
            Entry = entry;
            City = city;

            if (Entry != null)
            {
                Edit = true;
            }
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            AddHtmlLocalized(57, 155, 100, 20, 1158040, false, false); // City:
            AddHtmlLocalized(110, 155, 200, 20, CityLoyaltySystem.GetCityLocalization(City), false, false);

            AddHtmlLocalized(57, 185, 50, 20, 1158027, false, false); // Author:
            AddLabel(110, 185, 0, Entry != null ? Entry.Author : User.Name);

            AddHtmlLocalized(58, 215, 100, 20, 1158026, false, false); // Headline:
            AddBackground(58, 235, 740, 20, 0x2486);
            AddTextEntry(59, 235, 739, 20, 0, 1, Entry != null ? Entry.Title : "");

            AddHtmlLocalized(58, 265, 150, 20, 1158028, false, false); // Body Paragraph 1:
            AddBackground(58, 285, 740, 40, 0x2486);
            AddTextEntry(59, 285, 739, 40, 0, 2, Entry != null ? Entry.Body : "");

            AddBackground(155, 330, 20, 20, 0x2486);
            AddHtmlLocalized(58, 330, 150, 20, 1158031, false, false); // Expiry (in days):
            AddTextEntry(155, 330, 19, 20, 0, 3, "");

            AddImage(85, 425, 0x5EF);

            AddButton(40, 615, 0x601, 0x602, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(73, 615, 150, 20, 1077787, false, false); // Submit
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                string headline = info.GetTextEntry(1).Text;
                string body = info.GetTextEntry(2).Text;
                string exp = info.GetTextEntry(3).Text;

                int expires = Utility.ToInt32(exp);

                if (Entry == null)
                {
                    Entry = new TownCryerCityEntry(User, City, expires, headline, body);
                }
                else
                {
                    Entry.Title = headline;
                    Entry.Body = body;

                    if (expires >= 1 && expires <= 14)
                    {
                        Entry.Expires = DateTime.Now + TimeSpan.FromDays(expires);
                    }
                }

                if (expires < 1 || expires > 14)
                {
                    User.SendLocalizedMessage(1158042); // The expiry can be between 1 and 14 days. Please check your entry and try again.
                }
                else if (string.IsNullOrEmpty(headline) || string.IsNullOrEmpty(body) || headline.Length < 3 || body.Length < 5)
                {
                    User.SendLocalizedMessage(1158032); // The expiry can be between 1 and 30 days. Please check your entry and try again.
                }
                else
                {
                    if (!Edit)
                    {
                        TownCryerSystem.AddEntry(Entry);
                    }

                    User.SendLocalizedMessage(1158039); // Your entry has been submitted.

                    BaseGump.SendGump(new TownCryerGump(User, Cryer, 0, TownCryerGump.GumpCategory.City));
                    return;
                }

                Refresh();
            }
        }
    }
}