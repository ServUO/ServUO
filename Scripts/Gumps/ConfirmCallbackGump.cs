using Server.Mobiles;
using System;

namespace Server.Gumps
{
    public class ConfirmCallbackGump : BaseGump
    {
        public Action<Mobile, object> ConfirmCallback { get; set; }
        public Action<Mobile, object> CloseCallback { get; set; }

        public TextDefinition Title { get; set; }
        public TextDefinition Body { get; set; }
        public object State { get; set; }
        public string Arguments { get; set; }

        public int ConfirmLocalization { get; }
        public int CloseLocalization { get; }

        public ConfirmCallbackGump(
            PlayerMobile user,
            TextDefinition title,
            TextDefinition body,
            object state,
            string args = null,
            Action<Mobile, object> confirm = null,
            Action<Mobile, object> close = null,
            int x = 20,
            int y = 20,
            int confirmLoc = 1074976,
            int closeLoc = 1074977)
            : base(user, x, y)
        {
            Title = title;
            Body = body;
            State = state;
            Arguments = args;

            ConfirmCallback = confirm;
            CloseCallback = close;

            ConfirmLocalization = confirmLoc;
            CloseLocalization = closeLoc;

            if (!Open)
                AddGumpLayout();
        }

        public override void AddGumpLayout()
        {
            AddImageTiled(0, 0, 348, 262, 0xA8E);
            AddAlphaRegion(0, 0, 348, 262);
            AddImage(0, 15, 0x27A8);
            AddImageTiled(0, 30, 17, 200, 0x27A7);
            AddImage(0, 230, 0x27AA);
            AddImage(15, 0, 0x280C);
            AddImageTiled(30, 0, 300, 17, 0x280A);
            AddImage(315, 0, 0x280E);
            AddImage(15, 244, 0x280C);
            AddImageTiled(30, 244, 300, 17, 0x280A);
            AddImage(315, 244, 0x280E);
            AddImage(330, 15, 0x27A8);
            AddImageTiled(330, 30, 17, 200, 0x27A7);
            AddImage(330, 230, 0x27AA);
            AddImage(333, 2, 0x2716);
            AddImage(333, 248, 0x2716);
            AddImage(2, 248, 0x2716);
            AddImage(2, 2, 0x2716);

            if (Title != null)
            {
                if (Title.Number > 0)
                {
                    AddHtmlLocalized(25, 25, 200, 20, Title.Number, 0x7D00, false, false);
                }
                else if (!string.IsNullOrEmpty(Title.String))
                {
                    AddHtml(25, 25, 200, 20, string.Format("<basefont color=#FF0000>{0}", Title.String), false, false);
                }
            }

            AddImage(25, 45, 0xBBF);

            if (Body.Number > 0)
            {
                if (Arguments != null)
                {
                    AddHtmlLocalized(25, 55, 300, 120, Body.Number, Arguments, 0xFFFFFF, false, false);
                }
                else
                {
                    AddHtmlLocalized(25, 55, 300, 120, Body.Number, 0xFFFFFF, false, false);
                }
            }
            else if (!string.IsNullOrEmpty(Body.String))
            {
                AddHtml(25, 55, 300, 120, string.Format("<BASEFONT COLOR=#FFFFFF>{0}</BASEFONT>", Body.String), false, false);
            }

            AddRadio(25, 175, 0x25F8, 0x25FB, true, 1);
            AddRadio(25, 210, 0x25F8, 0x25FB, false, 2);

            AddHtmlLocalized(60, 180, 280, 20, ConfirmLocalization, 0xFFFFFF, false, false); // Yes
            AddHtmlLocalized(60, 215, 280, 20, CloseLocalization, 0xFFFFFF, false, false);   // No

            AddButton(265, 220, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID != 1)
                return;

            bool confirm = info.IsSwitched(1);

            if (confirm)
            {
                if (ConfirmCallback != null)
                {
                    ConfirmCallback(User, State);
                }
            }
            else if (CloseCallback != null)
            {
                CloseCallback(User, State);
            }
        }
    }

    public class GenericConfirmCallbackGump<T> : BaseGump
    {
        public Action<Mobile, T> ConfirmCallback { get; set; }
        public Action<Mobile, T> CloseCallback { get; set; }

        public TextDefinition Title { get; set; }
        public TextDefinition Body { get; set; }
        public T State { get; set; }
        public string Arguments { get; set; }

        public int ConfirmLocalization { get; }
        public int CloseLocalization { get; }

        public GenericConfirmCallbackGump(
            PlayerMobile user,
            TextDefinition title,
            TextDefinition body,
            T state,
            string args = null,
            Action<Mobile, T> confirm = null,
            Action<Mobile, T> close = null,
            int x = 20,
            int y = 20,
            int confirmLoc = 1074976,
            int closeLoc = 1074977)
            : base(user, x, y)
        {
            Title = title;
            Body = body;
            State = state;
            Arguments = args;

            ConfirmCallback = confirm;
            CloseCallback = close;

            ConfirmLocalization = confirmLoc;
            CloseLocalization = closeLoc;

            if (!Open)
                AddGumpLayout();
        }

        public override void AddGumpLayout()
        {
            AddImageTiled(0, 0, 348, 262, 0xA8E);
            AddAlphaRegion(0, 0, 348, 262);
            AddImage(0, 15, 0x27A8);
            AddImageTiled(0, 30, 17, 200, 0x27A7);
            AddImage(0, 230, 0x27AA);
            AddImage(15, 0, 0x280C);
            AddImageTiled(30, 0, 300, 17, 0x280A);
            AddImage(315, 0, 0x280E);
            AddImage(15, 244, 0x280C);
            AddImageTiled(30, 244, 300, 17, 0x280A);
            AddImage(315, 244, 0x280E);
            AddImage(330, 15, 0x27A8);
            AddImageTiled(330, 30, 17, 200, 0x27A7);
            AddImage(330, 230, 0x27AA);
            AddImage(333, 2, 0x2716);
            AddImage(333, 248, 0x2716);
            AddImage(2, 248, 0x2716);
            AddImage(2, 2, 0x2716);

            if (Title.Number > 0)
            {
                AddHtmlLocalized(25, 25, 200, 20, Title.Number, 0x7D00, false, false);
            }
            else if (!string.IsNullOrEmpty(Title.String))
            {
                AddHtml(25, 25, 200, 20, string.Format("<basefont color=#FF0000>{0}", Title.String), false, false);
            }

            AddImage(25, 45, 0xBBF);

            if (Body.Number > 0)
            {
                if (Arguments != null)
                {
                    AddHtmlLocalized(25, 55, 300, 120, Body.Number, Arguments, 0xFFFFFF, false, false);
                }
                else
                {
                    AddHtmlLocalized(25, 55, 300, 120, Body.Number, 0xFFFFFF, false, false);
                }
            }
            else if (!string.IsNullOrEmpty(Body.String))
            {
                AddHtml(25, 55, 300, 120, string.Format("<BASEFONT COLOR=#FFFFFF>{0}</BASEFONT>", Body.String), false, false);
            }

            AddRadio(25, 175, 0x25F8, 0x25FB, true, 1);
            AddRadio(25, 210, 0x25F8, 0x25FB, false, 2);

            AddHtmlLocalized(60, 180, 280, 20, ConfirmLocalization, 0xFFFFFF, false, false); // Yes
            AddHtmlLocalized(60, 215, 280, 20, CloseLocalization, 0xFFFFFF, false, false);   // No

            AddButton(265, 220, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID != 1)
                return;

            bool confirm = info.IsSwitched(1);

            if (confirm)
            {
                if (ConfirmCallback != null)
                {
                    ConfirmCallback(User, State);
                }
            }
            else if (CloseCallback != null)
            {
                CloseCallback(User, State);
            }
        }
    }
}
