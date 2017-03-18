using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using Server.Network;

namespace Server.Engines.VvV
{
    public class ExemptCitiesGump : Gump
    {
        public ExemptCitiesGump()
            : base(50, 50)
        {
            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            AddBackground(0, 0, 250, 300, 83);
            AddHtml(0, 15, 250, 60, "<basefont color=#FFFFFF><center>City Exempt List:<br>Check any cities that you do not want participating in VvV Battles.</center>", false, false);

            for (int i = 0; i < 8; i++)
            {
                VvVCity city = (VvVCity)i;
                int button = ViceVsVirtueSystem.Instance.ExemptCities.Contains(city) ? 211 : 210;

                AddButton(20, 80 + (i * 23), button, button, i + 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 80 + (i * 23), 200, 20, ViceVsVirtueSystem.GetCityLocalization(city), 0xFFFF, false, false);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int id = info.ButtonID;

            if (id == 0)
                return;

            var city = (VvVCity)id - 1;

            if (ViceVsVirtueSystem.Instance.ExemptCities.Contains(city))
                ViceVsVirtueSystem.Instance.ExemptCities.Remove(city);
            else
                ViceVsVirtueSystem.Instance.ExemptCities.Add(city);

            if (state.Gumps.Contains(this))
                state.Gumps.Remove(this);

            Entries.Clear();
            AddGumpLayout();

            state.Mobile.SendGump(this);
        }
    }
}