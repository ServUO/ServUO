using Server.Gumps;
using Server.Mobiles;
using System;

namespace Server.Engines.SeasonalEvents
{
    public class SeasonalEventGump : BaseGump
    {
        public SeasonalEventGump(PlayerMobile pm)
            : base(pm, 100, 100)
        {
            pm.CloseGump(typeof(SeasonalEventGump));
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 500, 600, 9300);

            AddHtml(0, 10, 500, 20, Center("Season Event Configuration"), false, false);

            int y = 60;

            AddHtml(10, 40, 190, 20, "System Name", false, false);
            AddHtml(200, 40, 75, 20, "Status", false, false);
            AddHtml(275, 40, 150, 20, "Season", false, false);
            AddHtml(400, 40, 50, 20, "Props", false, false);
            AddHtml(450, 40, 50, 20, "Edit", false, false);

            for (int i = 0; i < SeasonalEventSystem.Entries.Count; i++)
            {
                SeasonalEvent entry = SeasonalEventSystem.Entries[i];

                int hue = entry.IsActive() ? 167 : 137;

                AddLabel(10, y, hue, entry.Name);
                AddLabel(200, y, hue, entry.Status.ToString());

                if (entry.Status != EventStatus.Seasonal)
                {
                    AddLabel(275, y, hue, "N/A");
                }
                else
                {
                    DateTime end = new DateTime(DateTime.Now.Year, entry.MonthStart, entry.DayStart, 0, 0, 0) + TimeSpan.FromDays(entry.Duration);

                    if (entry.Duration > -1)
                    {
                        AddLabel(275, y, hue, string.Format("{0}/{1} - {2}/{3}", entry.MonthStart.ToString(), entry.DayStart.ToString(), end.Month.ToString(), end.Day.ToString()));
                    }
                    else
                    {
                        AddLabel(275, y, hue, string.Format("{0}/{1} - Completion", entry.MonthStart.ToString(), entry.DayStart.ToString()));
                    }
                }

                AddButton(400, y, 4029, 4030, i + 100, GumpButtonType.Reply, 0);
                AddButton(450, y, 4029, 4030, i + 10, GumpButtonType.Reply, 0);
                y += 25;
            }

            AddButton(10, 568, 4017, 4018, 1, GumpButtonType.Reply, 0);
            AddHtml(45, 568, 150, 20, "Restore Defaults", false, false);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            if (info.ButtonID >= 100)
            {
                int id = info.ButtonID - 100;
                Refresh();

                if (id >= 0 && id < SeasonalEventSystem.Entries.Count)
                {
                    var entry = SeasonalEventSystem.Entries[id];

                    User.SendGump(new PropertiesGump(User, entry));
                    User.SendMessage("You are viewing props for {0}", entry.Name);
                }
            }
            else if (info.ButtonID >= 10)
            {
                int id = info.ButtonID - 10;

                if (id >= 0 && id < SeasonalEventSystem.Entries.Count)
                {
                    var entry = SeasonalEventSystem.Entries[id];

                    if (entry.EventType == EventType.TreasuresOfTokuno)
                    {
                        User.CloseGump(typeof(ToTAdminGump));
                        User.SendGump(new ToTAdminGump());
                    }
                    else
                    {
                        SendGump(new EditEventGump(User, entry));
                    }
                }
            }
            else if (info.ButtonID == 1)
            {
                SeasonalEventSystem.ClearEntries();
                SeasonalEventSystem.LoadEntries();
                User.SendMessage("All event entries have been restored to default.");

                Refresh();
            }
        }
    }

    public class EditEventGump : BaseGump
    {
        public SeasonalEvent Entry { get; set; }

        private int _Month;
        private int _Day;
        private int _Duration;
        private EventStatus _Status;

        public EditEventGump(PlayerMobile pm, SeasonalEvent entry)
            : base(pm, 100, 100)
        {
            pm.CloseGump(typeof(EditEventGump));

            Entry = entry;

            _Month = entry.MonthStart;
            _Day = entry.DayStart;
            _Duration = entry.Duration;
            _Status = entry.Status;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 400, 250, 9300);

            AddHtml(0, 10, 400, 20, Center(Entry.Name), false, false);

            AddHtml(10, 40, 100, 20, "Start Month:", false, false);
            AddHtml(10, 62, 100, 20, "Start Day:", false, false);
            AddHtml(10, 84, 100, 20, "Duration [Days]:", false, false);
            AddHtml(10, 106, 100, 20, "Auto Activate:", false, false);

            AddHtml(120, 40, 250, 20, GetMonth(_Month), false, false);
            AddHtml(120, 62, 250, 20, _Day.ToString(), false, false);
            AddTextEntry(120, 84, 250, 20, 0, 0, _Duration.ToString());
            AddHtml(120, 106, 250, 20, _Status.ToString(), false, false);

            AddButton(335, 40, 4014, 4015, 1, GumpButtonType.Reply, 0);
            AddButton(367, 40, 4005, 4006, 2, GumpButtonType.Reply, 0);

            AddButton(335, 62, 4014, 4015, 3, GumpButtonType.Reply, 0);
            AddButton(367, 62, 4005, 4006, 4, GumpButtonType.Reply, 0);

            AddButton(335, 84, 4014, 4015, 5, GumpButtonType.Reply, 0);
            AddButton(367, 84, 4005, 4006, 6, GumpButtonType.Reply, 0);

            AddButton(335, 106, 4014, 4015, 7, GumpButtonType.Reply, 0);
            AddButton(367, 106, 4005, 4006, 8, GumpButtonType.Reply, 0);

            AddButton(5, 225, 4023, 4024, 9, GumpButtonType.Reply, 0);
            AddHtml(40, 225, 150, 20, "Apply", false, false);

            AddButton(365, 225, 4014, 4016, 10, GumpButtonType.Reply, 0);
            AddHtml(260, 225, 100, 20, AlignRight("Back"), false, false);
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: return;
                case 1:
                    if (_Month == 1)
                        _Month = 12;
                    else
                        _Month--;
                    break;
                case 2:
                    if (_Month == 12)
                        _Month = 1;
                    else
                        _Month++;
                    break;
                case 3:
                    if (_Day == 1)
                        _Day = GetDaysInMonth(_Month);
                    else
                        _Day--;
                    break;
                case 4:
                    if (_Day == GetDaysInMonth(_Month))
                        _Day = 1;
                    else
                        _Day++;
                    break;
                case 5:
                    if (_Duration == 1)
                        _Duration = 1;
                    else
                        _Duration--;

                    _Duration = Math.Min(365, _Duration);
                    break;
                case 6:
                    _Duration++;
                    break;
                case 7:
                    if (_Status == EventStatus.Inactive)
                        _Status = EventStatus.Seasonal;
                    else
                        _Status--;
                    break;
                case 8:
                    if (_Status == EventStatus.Seasonal)
                        _Status = EventStatus.Inactive;
                    else
                        _Status++;
                    break;
                case 9:
                    Entry.MonthStart = _Month;
                    Entry.DayStart = _Day;
                    Entry.Status = _Status;

                    TextRelay relay = info.GetTextEntry(0);

                    if (relay != null && !string.IsNullOrEmpty(relay.Text))
                    {
                        int duration = Utility.ToInt32(relay.Text);

                        if (duration > 0)
                        {
                            _Duration = Math.Min(365, duration);
                        }
                    }

                    Entry.Duration = _Duration;

                    SendGump(new SeasonalEventGump(User));

                    return;
                case 10:
                    SendGump(new SeasonalEventGump(User));
                    return;
            }

            Refresh();
        }

        private string GetMonth(int month)
        {
            switch (month)
            {
                default:
                case 1: return "January";
                case 2: return "February";
                case 3: return "March";
                case 4: return "April";
                case 5: return "May";
                case 6: return "June";
                case 7: return "July";
                case 8: return "August";
                case 9: return "September";
                case 10: return "October";
                case 11: return "November";
                case 12: return "Decemebr";
            }
        }

        private int GetDaysInMonth(int month)
        {
            switch (month)
            {
                default:
                case 1: return 31;
                case 2: return 28;
                case 3: return 31;
                case 4: return 30;
                case 5: return 31;
                case 6: return 30;
                case 7: return 31;
                case 8: return 31;
                case 9: return 30;
                case 10: return 31;
                case 11: return 30;
                case 12: return 31;
            }
        }
    }
}
