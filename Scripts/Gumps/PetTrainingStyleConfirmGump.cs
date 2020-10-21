using Server.Mobiles;

using System;

namespace Server.Gumps
{
    public class PetTrainingStyleConfirmGump : BaseGump
    {
        private readonly TextDefinition _Title;
        private readonly TextDefinition _Body;

        private Action ConfirmCallback { get; }
        private Action CancelCallback { get; }

        public PetTrainingStyleConfirmGump(PlayerMobile pm, TextDefinition title, TextDefinition body, Action confirmCallback, Action cancelCallback = null)
            : base(pm, 250, 50)
        {
            pm.CloseGump(GetType());

            _Title = title;
            _Body = body;
            ConfirmCallback = confirmCallback;
            CancelCallback = cancelCallback;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 454, 240, 0x24A4);

            if (_Title.Number > 0)
            {
                AddHtmlLocalized(35, 10, 400, 20, 1114513, string.Format("#{0}", _Title.Number.ToString()), C32216(0x0d0d0d), false, false);
            }
            else if(!string.IsNullOrEmpty(_Title.String))
            {
                AddHtml(35, 10, 400, 20, Center(_Title.String), false, false);
            }

            if (_Body.Number > 0)
            {
                AddHtmlLocalized(55, 60, 350, 145, _Body.Number, 0x4000, false, false);
            }
            else if (!string.IsNullOrEmpty(_Body.String))
            {
                AddHtml(55, 60, 350, 145, Color(C16232(0x4000), _Body.String), false, false);
            }

            AddECHandleInput();

            AddButton(70, 150, 0x9CC8, 0x9CC7, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(70, 153, 126, 25, 1114513, "#1046362", 0, false, false);

            AddECHandleInput();
            AddECHandleInput();

            AddButton(235, 150, 0x9CC8, 0x9CC7, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(235, 153, 126, 25, 1114513, "#1006045", 0, false, false);

            AddECHandleInput();
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (ConfirmCallback != null)
                {
                    ConfirmCallback();
                }

                OnConfirm();
            }
            else if (info.ButtonID == 2)
            {
                if (CancelCallback != null)
                {
                    CancelCallback();
                }

                OnCancel();
            }
        }

        public virtual void OnConfirm()
        {
        }

        public virtual void OnCancel()
        {
        }
    }
}
