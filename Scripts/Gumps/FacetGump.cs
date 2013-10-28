using System;
using Server.Network;

namespace Server.Gumps
{
    public class FacetGump : Gump
    {
        private Mobile m_Owner;
        public FacetGump(Mobile owner)
            : base(10, 10)
        {
            owner.CloseGump(typeof(FacetGump));

            int gumpX = 0;
            int gumpY = 0;
            // bool initialState = false;

            this.m_Owner = owner;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);

            gumpX = 0;
            gumpY = 0;
            this.AddBackground(gumpX, gumpY, 150, 150, 0xA3C);

            gumpX = 30;
            gumpY = 10;
            this.AddLabel(gumpX, gumpY, 965, "Felucca");

            gumpX = 30;
            gumpY = 30;
            this.AddLabel(gumpX, gumpY, 965, "Trammel");

            gumpX = 30;
            gumpY = 50;
            this.AddLabel(gumpX, gumpY, 965, "Ilshenar");

            gumpX = 30;
            gumpY = 70;
            this.AddLabel(gumpX, gumpY, 965, "Malas");

            gumpX = 30;
            gumpY = 90;
            this.AddLabel(gumpX, gumpY, 965, "Tokuno");

            gumpX = 30;
            gumpY = 110;
            this.AddLabel(gumpX, gumpY, 965, "TerMur");

            gumpX = 10;
            gumpY = 10;
            this.AddButton(gumpX, gumpY, 0xA9A, 0xA9A, 1, GumpButtonType.Reply, 0);

            gumpX = 10;
            gumpY = 30;
            this.AddButton(gumpX, gumpY, 0xA9A, 0xA9A, 2, GumpButtonType.Reply, 0);

            gumpX = 10;
            gumpY = 50;
            this.AddButton(gumpX, gumpY, 0xA9A, 0xA9A, 3, GumpButtonType.Reply, 0);

            gumpX = 10;
            gumpY = 70;
            this.AddButton(gumpX, gumpY, 0xA9A, 0xA9A, 4, GumpButtonType.Reply, 0);

            gumpX = 10;
            gumpY = 90;
            this.AddButton(gumpX, gumpY, 0xA9A, 0xA9A, 5, GumpButtonType.Reply, 0);

            gumpX = 10;
            gumpY = 110;
            this.AddButton(gumpX, gumpY, 0xA9A, 0xA9A, 6, GumpButtonType.Reply, 0);
        }

        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch( info.ButtonID )
            {
                case 1:
                    CityInfo city = new CityInfo("Britain", "Town Center", 1475, 1645, 20);

                    from.MoveToWorld(city.Location, Map.Felucca);
                    break;
                case 2:
                    CityInfo city1 = new CityInfo("Britain", "Town Center", 1475, 1645, 20);

                    from.MoveToWorld(city1.Location, Map.Trammel);
                    break;
                case 3:
                    CityInfo city2 = new CityInfo("Lakeshire", "Town Center", 1203, 1124, -25);

                    from.MoveToWorld(city2.Location, Map.Ilshenar);
                    break;
                case 4:
                    CityInfo city3 = new CityInfo("Luna", "Town Center", 989, 519, -50);

                    from.MoveToWorld(city3.Location, Map.Malas);
                    break;
                case 5: 
                    CityInfo city4 = new CityInfo("Zento", "Town Center", 735, 1257, 30);

                    from.MoveToWorld(city4.Location, Map.Tokuno);
                    break;
                case 6:
                    CityInfo city5 = new CityInfo("Test", "Test", 852, 3526, -43);

                    from.MoveToWorld(city5.Location, Map.TerMur);
                    break;
            }
        }
    }
}