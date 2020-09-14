using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using System;

namespace Server.Items
{
    public interface ICustomizableMessageItem
    {
        string[] Lines { get; set; }
    }

    public class AddCustomizableMessageGump : BaseGump
    {
        private readonly ICustomizableMessageItem _MessageItem;
        private readonly int TitleCliloc;
        private readonly int SubjectCliloc;

        public AddCustomizableMessageGump(PlayerMobile pm, ICustomizableMessageItem item, int title = 0, int subject = 0)
            : base(pm, 100, 100)
        {
            _MessageItem = item;
            TitleCliloc = title;
            SubjectCliloc = subject;
        }

        public override void AddGumpLayout()
        {
            string line1 = string.Empty;
            string line2 = string.Empty;
            string line3 = string.Empty;

            if (_MessageItem.Lines != null)
            {
                if (_MessageItem.Lines.Length > 0)
                    line1 = _MessageItem.Lines[0];

                if (_MessageItem.Lines.Length > 1)
                    line2 = _MessageItem.Lines[1];

                if (_MessageItem.Lines.Length > 2)
                    line3 = _MessageItem.Lines[2];
            }

            AddPage(0);

            AddBackground(0, 0, 420, 320, 0x2454);
            AddHtmlLocalized(10, 10, 400, 18, 1114513, string.Format("#{0}", TitleCliloc == 0 ? 1151680 : TitleCliloc), 0x4000, false, false); // Add Message
            AddHtmlLocalized(10, 37, 400, 90, SubjectCliloc == 0 ? 1151681 : SubjectCliloc, 0x14AA, false, false); // Enter up to three lines of personallized text.  you may enter up to 25 characters per line.

            AddHtmlLocalized(10, 136, 400, 16, 1150296, 0x14AA, false, false); // Line 1:
            AddBackground(10, 152, 400, 22, 0x2486);
            AddTextEntry(12, 154, 396, 18, 0x9C2, 0, line1, 25);

            AddHtmlLocalized(10, 178, 400, 16, 1150297, 0x14AA, false, false); // Line 2:
            AddBackground(10, 194, 400, 22, 0x2486);
            AddTextEntry(12, 196, 396, 18, 0x9C2, 1, line2, 25);

            AddHtmlLocalized(10, 220, 400, 16, 1150298, 0x14AA, false, false); // Line 3:
            AddBackground(10, 236, 400, 22, 0x2486);
            AddTextEntry(12, 238, 396, 18, 0x9C2, 2, line3, 25);

            AddButton(10, 290, 0xFAB, 0xFAC, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 290, 100, 20, 1150299, 0x10, false, false); // ACCEPT

            AddButton(380, 290, 0xFB4, 0xFB5, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(270, 290, 100, 20, 1114514, "#1150300 ", 0x10, false, false); // CANCEL
        }

        public override void OnResponse(RelayInfo info)
        {
            if (((Item)_MessageItem).Deleted)
                return;

            if (info.ButtonID == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_MessageItem.Lines.Length <= i)
                    {
                        break;
                    }

                    TextRelay text = info.GetTextEntry(i);
                    string s = text.Text;

                    if (s.Length > 25)
                        s = s.Substring(0, 25);

                    _MessageItem.Lines[i] = s;
                }

                if (_MessageItem is BaseAddon)
                {
                    foreach (AddonComponent comp in ((BaseAddon)_MessageItem).Components)
                    {
                        comp.InvalidateProperties();
                    }
                }
                else if (_MessageItem is Item)
                {
                    ((Item)_MessageItem).InvalidateProperties();
                }
            }

            if (_MessageItem is ValentineBear)
            {
                ((ValentineBear)_MessageItem).EditEnd = DateTime.UtcNow + TimeSpan.FromMinutes(10.0);
            }
        }
    }

    public class EditSign : ContextMenuEntry
    {
        private readonly ICustomizableMessageItem _MessageItem;
        private readonly PlayerMobile _From;

        public EditSign(ICustomizableMessageItem messageItem, PlayerMobile from)
            : base(1151817) // Edit Sign
        {
            _MessageItem = messageItem;
            _From = from;
        }

        public override void OnClick()
        {
            if (_MessageItem is Item)
            {
                if (_MessageItem is BaseAddon || ((Item)_MessageItem).IsChildOf(_From.Backpack))
                {
                    BaseGump.SendGump(new AddCustomizableMessageGump(_From, _MessageItem));
                }
                else
                {
                    _From.SendLocalizedMessage(1116249); // That must be in your backpack for you to use it.
                }
            }
        }
    }
}
