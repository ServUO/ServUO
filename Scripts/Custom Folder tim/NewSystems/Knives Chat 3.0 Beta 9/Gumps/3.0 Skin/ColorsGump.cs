using System;
using Server;
using Server.HuePickers;

namespace Knives.Chat3
{
    public class ColorsGump : GumpPlus
    {
        private Mobile c_Target;

        public Mobile Current { get { return (c_Target == null ? Owner : c_Target); } }

        public ColorsGump(Mobile m, Mobile targ)
            : base(m, 100, 100)
        {
            m.CloseGump(typeof(ColorsGump));

            c_Target = targ;
        }

        public ColorsGump(Mobile m)
            : this(m, null)
        {
        }

        protected override void BuildGump()
        {
            int width = 200;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(216));
            AddImage(width / 2 - 70, y + 2, 0x39);
            AddImage(width / 2 + 40, y + 2, 0x3B);

            if (c_Target != null)
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(224) + " " + c_Target.RawName);

            foreach (Channel c in Channel.Channels)
                if(c.CanChat(Current, false))
                {
                    AddHtml(0, y+=25, width, "<CENTER>" + c.NameFor(Current));
                    AddImage(width / 2 - 70, y, 0x2342, c.ColorFor(Current));
                    AddButton(width / 2 - 66, y + 4, 0x2716, "Channel Color", new GumpStateCallback(ChannelColor), c);
                    AddImage(width / 2 + 50, y, 0x2342, c.ColorFor(Current));
                    AddButton(width / 2 + 55, y + 4, 0x2716, "Channel Color", new GumpStateCallback(ChannelColor), c);
                }

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(47));
            AddImage(width / 2 - 70, y, 0x2342, Data.GetData(Current).SystemC);
            AddButton(width / 2 - 66, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 2);
            AddImage(width / 2 + 50, y, 0x2342, Data.GetData(Current).SystemC);
            AddButton(width / 2 + 55, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 2);

            if (Current.AccessLevel > AccessLevel.Player)
            {
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(48));
                AddImage(width / 2 - 70, y, 0x2342, Data.GetData(Current).StaffC);
                AddButton(width / 2 - 66, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 3);
                AddImage(width / 2 + 50, y, 0x2342, Data.GetData(Current).StaffC);
                AddButton(width / 2 + 55, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 3);
            }

            if (Data.GetData(Current).GlobalAccess)
            {
                y += 20;

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(44));
                AddImage(width / 2 - 70, y, 0x2342, Data.GetData(Current).GlobalCC);
                AddButton(width / 2 - 66, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 0);
                AddImage(width / 2 + 50, y, 0x2342, Data.GetData(Current).GlobalCC);
                AddButton(width / 2 + 55, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 0);

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(45));
                AddImage(width / 2 - 70, y, 0x2342, Data.GetData(Current).GlobalWC);
                AddButton(width / 2 - 66, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 1);
                AddImage(width / 2 + 50, y, 0x2342, Data.GetData(Current).GlobalWC);
                AddButton(width / 2 + 55, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 1);

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(192));
                AddImage(width / 2 - 70, y, 0x2342, Data.GetData(Current).GlobalGC);
                AddButton(width / 2 - 66, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 4);
                AddImage(width / 2 + 50, y, 0x2342, Data.GetData(Current).GlobalGC);
                AddButton(width / 2 + 55, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 4);

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(193));
                AddImage(width / 2 - 70, y, 0x2342, Data.GetData(Current).GlobalFC);
                AddButton(width / 2 - 66, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 5);
                AddImage(width / 2 + 50, y, 0x2342, Data.GetData(Current).GlobalFC);
                AddButton(width / 2 + 55, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 5);

                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(26));
                AddImage(width / 2 - 70, y, 0x2342, Data.GetData(Current).GlobalMC);
                AddButton(width / 2 - 66, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 6);
                AddImage(width / 2 + 50, y, 0x2342, Data.GetData(Current).GlobalMC);
                AddButton(width / 2 + 55, y + 4, 0x2716, "Colors", new GumpStateCallback(Colors), 6);
            }

            AddBackgroundZero(0, 0, width, y+40, Data.GetData(Current).DefaultBack);
        }

        private void ChannelColor(object o)
        {
            if (!(o is Channel))
                return;

            Owner.SendHuePicker(new InternalPicker(this, (Channel)o));
        }

        private void Colors(object o)
        {
            if (!(o is int))
                return;

            Owner.SendHuePicker(new InternalPicker(this, (int)o));
        }


        private class InternalPicker : HuePicker
        {
            private ColorsGump c_Gump;
            private int c_Num;
            private Channel c_Channel;

            public InternalPicker(ColorsGump g, int num)
                : base(0x1018)
            {
                c_Gump = g;
                c_Num = num;
            }

            public InternalPicker(ColorsGump g, Channel c)
                : base(0x1018)
            {
                c_Gump = g;
                c_Channel = c;
            }

            public override void OnResponse(int hue)
            {
                if (c_Channel != null)
                {
                    c_Channel.Colors[c_Gump.Current] = hue;
                    c_Gump.NewGump();
                    return;
                }

                switch (c_Num)
                {
                    case 0: Data.GetData(c_Gump.Current).GlobalCC = hue; break;
                    case 1: Data.GetData(c_Gump.Current).GlobalWC = hue; break;
                    case 2: Data.GetData(c_Gump.Current).SystemC = hue; break;
                    case 3: Data.GetData(c_Gump.Current).StaffC = hue; break;
                    case 4: Data.GetData(c_Gump.Current).GlobalGC = hue; break;
                    case 5: Data.GetData(c_Gump.Current).GlobalFC = hue; break;
                    case 6: Data.GetData(c_Gump.Current).GlobalMC = hue; break;
                }

                c_Gump.NewGump();
            }
        }
    }
}