using System;

namespace Server.Gumps
{
    public class SaveCancelGumpling : Gumpling
    {
        public event GumpResponse OnSavePressed;
        public event GumpResponse OnCancelPressed;

        public SaveCancelGumpling(Int32 x, Int32 y) : this(x, y, null, null) { }

        public SaveCancelGumpling(Int32 x, Int32 y, GumpResponse saveCallback) : this(x, y, saveCallback, null) { }

        public SaveCancelGumpling(Int32 x, Int32 y, GumpResponse saveCallback, GumpResponse cancelCallback) : base(x, y)
        {
            GumpButton button = new GumpButton(0, 0, 0x1452, 0x1453, -1, GumpButtonType.Reply, 0, saveCallback);
            button.OnGumpResponse += button_OnSave;
            Add(button);

            button = new GumpButton(85, 0, 0x1450, 0x1451, (cancelCallback == null ? 0 : -1), GumpButtonType.Reply, 0, cancelCallback);
            button.OnGumpResponse += button_OnCancel;
            Add(button);
        }

        private void button_OnSave(IGumpComponent sender, object param)
        {
            if (OnSavePressed != null)
                OnSavePressed(sender, param);
        }

        private void button_OnCancel(IGumpComponent sender, object param)
        {
            if (OnCancelPressed != null)
                OnCancelPressed(sender, param);
        }
    }
}
