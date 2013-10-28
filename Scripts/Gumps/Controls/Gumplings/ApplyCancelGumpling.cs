using System;

namespace Server.Gumps
{
    public class ApplyCancelGumpling : Gumpling
    {
        public event GumpResponse OnApplyPressed;
        public event GumpResponse OnCancelPressed;

        public ApplyCancelGumpling(Int32 x, Int32 y) : this(x, y, null, null) { }

        public ApplyCancelGumpling(Int32 x, Int32 y, GumpResponse applyCallback, GumpResponse cancelCallback) : base(x, y)
        {
            GumpButton button = new GumpButton(0, 0, 0x1454, 0x1455, -1, GumpButtonType.Reply, 0, applyCallback);
            button.OnGumpResponse += button_OnApply;
            Add(button);

            button = new GumpButton(85, 0, 0x1450, 0x1451, 0, GumpButtonType.Reply, 0, cancelCallback);
            button.OnGumpResponse += button_OnCancel;
            Add(button);
        }

        private void button_OnApply(IGumpComponent sender, object param)
        {
            if (OnApplyPressed != null)
                OnApplyPressed(sender, param);
        }

        private void button_OnCancel(IGumpComponent sender, object param)
        {
            if (OnCancelPressed != null)
                OnCancelPressed(sender, param);
        }
    }
}
