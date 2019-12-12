using Server;
using System;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Services.TownCryer
{
    public class CreateGuildEntryGump : BaseTownCryerGump
    {
        public TownCryerGuildEntry Entry { get; set; }
        public bool Edit { get; private set; }

        public CreateGuildEntryGump(PlayerMobile pm, TownCrier cryer, TownCryerGuildEntry entry = null)
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
            AddLabel(105, 150, 0, String.Format("{0}", Entry != null ? Entry.Author : User.Name));

            AddHtmlLocalized(58, 170, 100, 20, 1158055, false, false); // Guild:
            AddLabel(105, 170, 0, Entry != null && Entry.Guild != null ? Entry.Guild.Name : "Unknown");

            AddHtmlLocalized(58, 190, 100, 20, 1158026, false, false); // Headline:
            AddBackground(58, 210, 740, 20, 0x2486);
            AddTextEntry(59, 210, 739, 20, 0, 1, Entry != null ? Entry.Title : "");

            AddBackground(138, 240, 20, 20, 0x2486);
            AddHtmlLocalized(58, 240, 100, 20, 1158056, false, false); // Event Month:
            AddTextEntry(139, 240, 19, 20, 0, 2, Entry != null ? Entry.EventTime.Month.ToString() : "", 2);

            AddBackground(323, 240, 20, 20, 0x2486);
            AddHtmlLocalized(258, 240, 150, 20, 1158057, false, false); // Event Day:
            AddTextEntry(324, 240, 19, 20, 0, 3, Entry != null ? Entry.EventTime.Day.ToString() : "", 2);

            AddBackground(529, 240, 20, 20, 0x2486);
            AddHtmlLocalized(458, 240, 150, 20, 1158058, false, false); // Event Time:
            AddTextEntry(530, 240, 19, 20, 0, 4, Entry != null ? Entry.EventTime.Hour.ToString() : "", 2);

            AddHtmlLocalized(58, 260, 150, 20, 1158059, false, false); // Event Timezone:
            AddLabel(155, 260, 0, TimeZone.CurrentTimeZone.StandardName);

            AddHtmlLocalized(58, 290, 150, 20, 1158060, false, false); // Event Description:
            AddBackground(58, 310, 740, 40, 0x2486);
            AddTextEntry(59, 310, 739, 40, 0, 5, Entry != null ? Entry.Body : "");

            AddHtmlLocalized(58, 370, 150, 20, 1158061, false, false); // Event Meeting Place:
            AddBackground(58, 390, 740, 40, 0x2486);
            AddTextEntry(59, 390, 739, 40, 0, 6, Entry != null ? Entry.EventLocation : "");

            AddImage(85, 425, 0x5EF);

            AddButton(40, 615, 0x601, 0x602, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(73, 615, 150, 20, 1077787, false, false); // Submit
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                string headline = info.GetTextEntry(1).Text;
                string m = info.GetTextEntry(2).Text;
                string d = info.GetTextEntry(3).Text;
                string t = info.GetTextEntry(4).Text;
                string desc = info.GetTextEntry(5).Text;
                string meet = info.GetTextEntry(6).Text;

                DateTime dt = DateTime.Now;
                bool illegalDate = false;

                int year = dt.Year;

                if (Utility.ToInt32(m) < DateTime.Now.Month)
                {
                    year++;
                }

                if (Entry == null)
                {
                    if (!DateTime.TryParse(String.Format("{0}/{1}/{2} {3}:00:00", m, d, year.ToString(), t), out dt)) // bad format
                    {
                        illegalDate = true;
                        dt = DateTime.MinValue;
                    }

                    Entry = new TownCryerGuildEntry(User, dt, meet, headline, desc);
                }
                else
                {
                    Entry.Title = headline;
                    Entry.Body = desc;
                    Entry.EventLocation = meet;

                    if (DateTime.TryParse(String.Format("{0}/{1}/{2} {3}:00:00", m, d, year.ToString(), t), out dt))
                    {
                        Entry.EventTime = dt;
                    }
                }

                if (string.IsNullOrEmpty(headline) || string.IsNullOrEmpty(desc) || string.IsNullOrEmpty(meet))
                {
                    User.SendLocalizedMessage(1158062); // All fields must be populated.  Please check your entries and try again.
                }
                else if (illegalDate)
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

                    BaseGump.SendGump(new TownCryerGump(User, Cryer, 0, TownCryerGump.GumpCategory.Guild));
                    return;
                }

                Refresh();
            }
        }
    }
}