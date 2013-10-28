using System;

namespace Server.Gumps
{
    public class LabelAddRemoveGumpling : Gumpling
    {
        public event GumpResponse OnEdit;
        public event GumpResponse OnRemove;

        private Int32 _Index;
        public Int32 Index { get { return _Index; } }

        public LabelAddRemoveGumpling(Int32 index, Int32 x, Int32 y, Int32 entryWidth, String entryName) : this(index, x, y, entryWidth, entryName, 0x0, null, null) { }

        public LabelAddRemoveGumpling(Int32 index, Int32 x, Int32 y, Int32 entryWidth, String entryName, Int32 entryHue) : this(index, x, y, entryWidth, entryName, entryHue, null, null) { }

        public LabelAddRemoveGumpling(Int32 index, Int32 x, Int32 y, Int32 entryWidth, String entryName, GumpResponse editCallback, GumpResponse removeCallback) : this(index, x, y, entryWidth, entryName, 0x0, editCallback, removeCallback) { }

        public LabelAddRemoveGumpling(Int32 index, Int32 x, Int32 y, Int32 entryWidth, String entryName, Int32 entryHue, GumpResponse editCallback, GumpResponse removeCallback) : base(x, y)
        {
            _Index = index;

            Add(new GumpImageTiled(0, 0, entryWidth, 23, 0xA40));
            Add(new GumpImageTiled(1, 1, entryWidth - 2, 21, 0xBBC));
            Add(new GumpLabelCropped(5, 1, entryWidth - 10, 16, entryHue, entryName));

            GumpButton editButton = new GumpButton(entryWidth + 5, 0, 0xFBD, 0xFBF, -1, GumpButtonType.Reply, 0, editCallback);
            editButton.OnGumpResponse += editButton_OnGumpResponse;
            Add(editButton);

            GumpButton removeButton = new GumpButton(entryWidth + 35, 0, 0xFB4, 0xFB6, -1, GumpButtonType.Reply, 0, removeCallback);
            removeButton.OnGumpResponse += removeButton_OnGumpResponse;
            Add(removeButton);
        }

        void editButton_OnGumpResponse(IGumpComponent sender, object param)
        {
            if (OnEdit != null)
                OnEdit(sender, param);
        }

        void removeButton_OnGumpResponse(IGumpComponent sender, object param)
        {
            if (OnRemove != null)
                OnRemove(sender, param);
        }
    }
}
