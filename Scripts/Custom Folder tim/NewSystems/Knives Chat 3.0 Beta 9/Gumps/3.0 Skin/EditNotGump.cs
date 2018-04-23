using System;
using Server;

namespace Knives.Chat3
{
    public class EditNotGump : GumpPlus
    {
        private Notification c_Not;

        public EditNotGump(Mobile m, Notification not)
            : base(m, 100, 100)
        {
            c_Not = not;
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddTextField(width / 2 -50, y, 100, 21, 0x480, 0xBBC, "Name", c_Not.Name);
            AddButton(width / 2 - 90, y+3, 0x2716, "Name", new GumpCallback(Name));
            AddButton(width / 2 + 70, y+3, 0x2716, "Name", new GumpCallback(Name));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(272) + " | " + General.Local(273));
            AddButton(width / 2 - 60, y + 3, !c_Not.Gump ? 0x939 : 0x2716, "NotGump", new GumpCallback(NotGump));
            AddButton(width / 2 + 50, y + 3, c_Not.Gump ? 0x939 : 0x2716, "Gump", new GumpCallback(Gump));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(274));
            AddButton(width / 2 - 80, y + 3, 0x2716, "Set Time", new GumpCallback(SetTime));
            AddButton(width / 2 + 70, y + 3, 0x2716, "Set Time", new GumpCallback(SetTime));
            AddHtml(0, y += 25, width, "<CENTER>" + (c_Not.Recur.Days != 0 ? c_Not.Recur.Days + " days " : "") + (c_Not.Recur.Hours != 0 ? c_Not.Recur.Hours + "h " : "") + (c_Not.Recur.Minutes != 0 ? c_Not.Recur.Minutes + "m " : "") + (c_Not.Recur.Seconds != 0 ? c_Not.Recur.Seconds + "s" : ""));

            if (c_Not.Gump)
            {
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(275));
                AddButton(width / 2 - 90, y, c_Not.AntiMacro ? 0x2343 : 0x2342, "Anti Macro", new GumpCallback(AntiMacro));
                AddButton(width / 2 + 70, y, c_Not.AntiMacro ? 0x2343 : 0x2342, "Anti Macro", new GumpCallback(AntiMacro));
            }

            if (c_Not.AntiMacro)
            {
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(280));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(155) + " | " + General.Local(281));
                AddButton(width / 2 - 60, y + 3, Data.MacroPenalty == MacroPenalty.None ? 0x939 : 0x2716, "None", new GumpCallback(None));
                AddButton(width / 2 + 50, y + 3, Data.MacroPenalty == MacroPenalty.Kick ? 0x939 : 0x2716, "Kick", new GumpCallback(Kick));

                AddHtml(0, y += 25, width/2-20, "<DIV ALIGN=LEFT>" + General.Local(282));
                AddTextField(width / 2 + 20, y, 50, 21, 0x480, 0xBBC, "Delay", "" + Data.AntiMacroDelay);
                AddHtml(width/2+80, y += 25, 10, "s");
                AddButton(width / 2 + 90, y + 3, 0x2716, "Delay", new GumpCallback(Delay));
            }

            AddTextField(20, y += 25, width - 40, 80, 0x480, 0xBBC, "Text", c_Not.Text);
            AddButton(width / 2 - 32, y += 90, 0x98B, "Text", new GumpCallback(Text));
            AddHtml(width / 2 - 32 + 6, y + 3, 51, "<CENTER>" + General.Local(245));

            AddBackgroundZero(0, 0, width, y + 40, Data.GetData(Owner).DefaultBack);
        }

        private void Name()
        {
            c_Not.Name = GetTextField("Name");
            NewGump();
        }

        private void NotGump()
        {
            c_Not.Gump = false;
            c_Not.AntiMacro = false;
            NewGump();
        }

        private void Gump()
        {
            c_Not.Gump = true;
            NewGump();
        }

        private void AntiMacro()
        {
            c_Not.AntiMacro = !c_Not.AntiMacro;

            if (c_Not.AntiMacro)
                c_Not.Gump = true;

            NewGump();
        }

        private void Text()
        {
            c_Not.Text = GetTextField("Text");
            NewGump();
        }

        private void Delay()
        {
            Data.AntiMacroDelay = GetTextFieldInt("Delay");
            NewGump();
        }

        private void SetTime()
        {
            new TimeGump(Owner, this, c_Not);
        }

        private void None()
        {
            Data.MacroPenalty = MacroPenalty.None;
            NewGump();
        }

        private void Kick()
        {
            Data.MacroPenalty = MacroPenalty.Kick;
            NewGump();
        }


        private class TimeGump : GumpPlus
        {
            private GumpPlus c_Gump;
            private Notification c_Not;

            public TimeGump(Mobile m, GumpPlus g, Notification not)
                : base(g.Owner, 100, 100)
            {
                c_Gump = g;
                c_Not = not;
            }

            protected override void BuildGump()
            {
                int width = 200;
                int y = 10;

                AddHtml(0, y, width, "<CENTER>" + General.Local(274));
                AddImage(width / 2 - 70, y + 2, 0x39);
                AddImage(width / 2 + 40, y + 2, 0x3B);

                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(161));
                AddButton(width / 2 - 60, y + 3, 0x2716, "30 minutes", new GumpStateCallback(Time), TimeSpan.FromMinutes(30));
                AddButton(width / 2 + 50, y + 3, 0x2716, "30 minutes", new GumpStateCallback(Time), TimeSpan.FromMinutes(30));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(162));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 hour", new GumpStateCallback(Time), TimeSpan.FromHours(1));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 hour", new GumpStateCallback(Time), TimeSpan.FromHours(1));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(163));
                AddButton(width / 2 - 60, y + 3, 0x2716, "12 hours", new GumpStateCallback(Time), TimeSpan.FromHours(12));
                AddButton(width / 2 + 50, y + 3, 0x2716, "12 hours", new GumpStateCallback(Time), TimeSpan.FromHours(12));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(164));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 day", new GumpStateCallback(Time), TimeSpan.FromDays(1));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 day", new GumpStateCallback(Time), TimeSpan.FromDays(1));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(165));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 week", new GumpStateCallback(Time), TimeSpan.FromDays(7));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 week", new GumpStateCallback(Time), TimeSpan.FromDays(7));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(166));
                AddButton(width / 2 - 60, y + 3, 0x2716, "1 month", new GumpStateCallback(Time), TimeSpan.FromDays(30));
                AddButton(width / 2 + 50, y + 3, 0x2716, "1 month", new GumpStateCallback(Time), TimeSpan.FromDays(30));

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(Owner).DefaultBack);
            }

            private void Time(object o)
            {
                if (!(o is TimeSpan))
                    return;

                c_Not.Recur = (TimeSpan)o;

                c_Gump.NewGump();
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }
        }
    }
}