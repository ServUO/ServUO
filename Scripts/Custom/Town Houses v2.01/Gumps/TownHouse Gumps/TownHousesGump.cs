using System;
using System.Collections;
using Server;
using Server.Multis;
using Server.Gumps;

namespace Knives.TownHouses
{
    public class TownHousesGump : GumpPlusLight
    {
        public enum ListPage { Town, House }

        public static void Initialize()
        {
            RUOVersion.AddCommand("TownHouses", AccessLevel.Counselor, new TownHouseCommandHandler(OnHouses));
        }

        private static void OnHouses(CommandInfo info)
        {
            new TownHousesGump(info.Mobile);
        }

        private ListPage c_ListPage;
        private int c_Page;

        public TownHousesGump(Mobile m)
            : base(m, 100, 100)
        {
            m.CloseGump(typeof(TownHousesGump));
        }

        protected override void BuildGump()
        {
            int width = 400;
            int y = 0;

            AddHtml(0, y += 10, width, "<CENTER>TownHouses Main Menu");
            AddImage(width / 2 - 120, y + 2, 0x39);
            AddImage(width / 2 + 90, y + 2, 0x3B);

            int pp = 10;

            if (c_Page != 0)
                AddButton(width / 2 - 10, y += 25, 0x25E4, 0x25E5, "Page Down", new GumpCallback(PageDown));

            ArrayList list = new ArrayList();
            if(c_ListPage == ListPage.Town)
                list = new ArrayList(TownHouseSign.AllSigns);
            else
                foreach(Item item in World.Items.Values)
                    if(item is BaseHouse)
                        list.Add(item);

            list.Sort(new InternalSort());

            AddHtml(0, y += 20, width, "<CENTER>" + (c_ListPage == ListPage.Town ? "TownHouses" : "Houses") + " Count: " + list.Count);
            AddHtml(0, y += 25, width, "<CENTER>TownHouses / Houses");
            AddButton(width / 2 - 100, y + 3, c_ListPage == ListPage.Town ? 0x939 : 0x2716, "Page", new GumpStateCallback(Page), ListPage.Town);
            AddButton(width / 2 + 90, y + 3, c_ListPage == ListPage.House ? 0x939 : 0x2716, "Page", new GumpStateCallback(Page), ListPage.House);

            TownHouseSign sign = null;
            BaseHouse house = null;

            y += 5;

            for (int i = c_Page * pp; i < (c_Page + 1) * pp && i < list.Count; ++i)
            {
                if (c_ListPage == ListPage.Town)
                {
                    sign = (TownHouseSign)list[i];

                    AddHtml(30, y += 20, width / 2 - 20, "<DIV ALIGN=LEFT>" + sign.Name);
                    AddButton(15, y + 3, 0x2716, "TownHouse Menu", new GumpStateCallback(TownHouseMenu), sign);

                    if (sign.House != null && sign.House.Owner != null)
                    {
                        AddHtml(width / 2, y, width / 2 - 40, "<DIV ALIGN=RIGHT>" + sign.House.Owner.RawName);
                        AddButton(width - 30, y + 3, 0x2716, "House Menu", new GumpStateCallback(HouseMenu), sign.House);
                    }
                }
                else
                {
                    house = (BaseHouse)list[i];

                    AddHtml(30, y += 20, width / 2 - 20, "<DIV ALIGN=LEFT>" + house.Name);
                    AddButton(15, y + 3, 0x2716, "Goto", new GumpStateCallback(Goto), house);

                    if (house.Owner != null)
                    {
                        AddHtml(width / 2, y, width / 2 - 40, "<DIV ALIGN=RIGHT>" + house.Owner.RawName);
                        AddButton(width - 30, y + 3, 0x2716, "House Menu", new GumpStateCallback(HouseMenu), house);
                    }
                }
            }

            if (pp * (c_Page + 1) < list.Count)
                AddButton(width / 2 - 10, y += 25, 0x25E8, 0x25E9, "Page Up", new GumpCallback(PageUp));

            if (c_ListPage == ListPage.Town)
            {
                AddHtml(0, y += 35, width, "<CENTER>Add New TownHouse");
                AddButton(width / 2 - 80, y + 3, 0x2716, "New", new GumpCallback(New));
                AddButton(width / 2 + 70, y + 3, 0x2716, "New", new GumpCallback(New));
            }

            AddBackgroundZero(0, 0, width, y + 40, 0x13BE);
        }

        private void TownHouseMenu(object obj)
        {
            if (!(obj is TownHouseSign))
                return;

            NewGump();

            new TownHouseSetupGump(Owner, (TownHouseSign)obj);
        }

        private void Page(object obj)
        {
            c_ListPage = (ListPage)obj;
            NewGump();
        }

        private void Goto(object obj)
        {
            if (!(obj is BaseHouse))
                return;

            Owner.Location = ((BaseHouse)obj).BanLocation;
            Owner.Map = ((BaseHouse)obj).Map;

            NewGump();
        }

        private void HouseMenu(object obj)
        {
            if (!(obj is BaseHouse))
                return;

            NewGump();

            Owner.SendGump(new HouseGumpAOS((HouseGumpPageAOS)0, Owner, (BaseHouse)obj));
        }

        private void New()
        {
            TownHouseSign sign = new TownHouseSign();
            Owner.AddToBackpack(sign);
            Owner.SendMessage("A new sign is now in your backpack.  It will move on it's own during setup, but if you don't complete setup you may want to delete it.");

            NewGump();

            new TownHouseSetupGump(Owner, sign);
        }

        private void PageUp()
        {
            c_Page++;
            NewGump();
        }

        private void PageDown()
        {
            c_Page--;
            NewGump();
        }


        private class InternalSort : IComparer
        {
            public InternalSort()
            {
            }

            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                    return 0;

                if (x is TownHouseSign)
                {
                    TownHouseSign a = (TownHouseSign)x;
                    TownHouseSign b = (TownHouseSign)y;

                    return Insensitive.Compare(a.Name, b.Name);
                }
                else
                {
                    BaseHouse a = (BaseHouse)x;
                    BaseHouse b = (BaseHouse)y;

                    if (a.Owner == null && b.Owner != null)
                        return -1;
                    if (a.Owner != null && b.Owner == null)
                        return 1;

                    return Insensitive.Compare(a.Owner.RawName, b.Owner.RawName);
                }
            }
        }
    }
}