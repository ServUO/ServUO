using System;
using Server;

namespace Knives.Chat3
{
    public enum FilterPenalty { None, Ban, Jail }

	public class Filter
	{
        public static string FilterText(Mobile m, string s)
        {
            return FilterText(m, s, true);
        }

        public static string FilterText(Mobile m, string s, bool punish)
        {
            try
            {
                string filter = "";
                string subOne = "";
                string subTwo = "";
                string subThree = "";
                int index = 0;

                for (int i = 0; i < Data.Filters.Count; ++i)
                {
                    filter = Data.Filters[i].ToString();

                    if (filter == "")
                    {
                        Data.Filters.Remove(filter);
                        continue;
                    }

                    index = s.ToLower().IndexOf(filter);

                    if (index >= 0)
                    {
                        if (m.AccessLevel == AccessLevel.Player && punish)
                        {
                            if (++Data.GetData(m).Warnings <= Data.FilterWarnings)
                                m.SendMessage(Data.GetData(m).SystemC, General.Local(111) + " " + filter);
                            else
                            {
                                Data.GetData(m).Warnings = 0;

                                Events.InvokeFilterViolation(new FilterViolationEventArgs(m));

                                if (Data.FilterPenalty == FilterPenalty.Ban)
                                    Data.GetData(m).Ban(TimeSpan.FromMinutes(Data.FilterBanLength));

                                if (Data.FilterPenalty == FilterPenalty.Jail)
                                    ChatJail.SendToJail(m);

                                if (Data.FilterPenalty != FilterPenalty.None)
                                    return "";
                            }
                        }

                        subOne = s.Substring(0, index);
                        subTwo = "";

                        for (int ii = 0; ii < filter.Length; ++ii)
                            subTwo += "*";

                        subThree = s.Substring(index + filter.Length, s.Length - filter.Length - index);

                        s = subOne + subTwo + subThree;

                        i--;
                    }
                }

            }
            catch { Errors.Report(General.Local(176)); }

            return s;
        }
    }
}