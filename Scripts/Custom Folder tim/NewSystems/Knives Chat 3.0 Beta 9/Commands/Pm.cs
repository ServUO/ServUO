using System;
using System.Collections;
using Server;
using Server.Gumps;

namespace Knives.Chat3
{
    public class Pm
    {
        public static void Initialize()
        {
            RUOVersion.AddCommand("pm", AccessLevel.Player, new ChatCommandHandler(OnMessage));
            RUOVersion.AddCommand("msg", AccessLevel.Player, new ChatCommandHandler(OnMessage));
        }

        private static void OnMessage(CommandInfo e)
        {
            if (e.ArgString == null || e.ArgString == "")
                return;

            string name = e.GetString(0);
            string text = "";

            if (e.Arguments.Length > 1)
                text = e.ArgString.Substring(name.Length + 1, e.ArgString.Length - name.Length - 1);

            ArrayList list = GetMsgCanidates(e.Mobile, name);

            if (list.Count > 10)
                e.Mobile.SendMessage(Data.GetData(e.Mobile).SystemC, General.Local(112));
            else if (list.Count == 0)
                e.Mobile.SendMessage(Data.GetData(e.Mobile).SystemC, General.Local(113));
            else if (list.Count == 1)
                new SendMessageGump(e.Mobile, (Mobile)list[0], text, null, MsgType.Normal);
            else
                new InternalGump(e.Mobile, list, text);
        }

        private static ArrayList GetMsgCanidates(Mobile m, string name)
        {
            ArrayList list = new ArrayList();

            foreach (Data data in new ArrayList(Data.Datas.Values))
                if (data.Mobile.RawName.ToLower().IndexOf(name.ToLower()) != -1 && Message.CanMessage(m, data.Mobile))
                    list.Add(data.Mobile);

            return list;
        }


        private class InternalGump : GumpPlus
        {
            private ArrayList c_List;
            private string c_Text;

            public InternalGump(Mobile m, ArrayList list, string txt) : base(m, 100, 100)
            {
                c_List = list;
                c_Text = txt;
            }

            protected override void BuildGump()
            {
                int y = 10;

                AddHtml(0, y, 150, 21, HTML.White + "<CENTER>" + General.Local(114), false, false);

                y += 5;

                foreach (Mobile m in c_List)
                {
                    AddHtml(60, y += 20, 90, 21, HTML.White + m.RawName, false, false);
                    AddButton(45, y + 3, 0x2716, 0x2716, "Select", new GumpStateCallback(Select), m);
                }

                AddBackgroundZero(0, 0, 150, y + 40, 0x1400);
            }

            private void Select(object o)
            {
                if (!(o is Mobile))
                    return;

                 new SendMessageGump(Owner, (Mobile)o, c_Text, null, MsgType.Normal);
            }
        }
    }
}