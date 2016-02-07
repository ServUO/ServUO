using System;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class ConfirmHeritageGump : Gump
    {
        private readonly HeritageToken m_Token;
        private readonly Type[] m_Selected;
		private readonly Mobile m_User;
        public ConfirmHeritageGump(HeritageToken token, Type[] selected, int cliloc, Mobile from)
            : base(60, 36)
        {
            this.m_Token = token;
            this.m_Selected = selected;
			this.m_User = from;

            this.AddPage(0);

            this.AddBackground(0, 0, 291, 99, 0x13BE);
            this.AddImageTiled(5, 6, 280, 20, 0xA40);
            this.AddHtmlLocalized(9, 8, 280, 20, 1070972, 0x7FFF, false, false); // Click "OKAY" to redeem the following promotional item:
            this.AddImageTiled(5, 31, 280, 40, 0xA40);
            this.AddHtmlLocalized(9, 35, 272, 40, cliloc, 0x7FFF, false, false);
            this.AddButton(180, 73, 0xFB7, 0xFB8, (int)Buttons.Okay, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(215, 75, 100, 20, 1011036, 0x7FFF, false, false); // OKAY
            this.AddButton(5, 73, 0xFB1, 0xFB2, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(40, 75, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
        }

        private enum Buttons
        {
            Cancel,
            Okay
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Token == null || this.m_Token.Deleted ||
				this.m_User == null || this.m_User.Deleted)
                return;

			if (!this.m_Token.IsChildOf(this.m_User.Backpack))
			{
				sender.Mobile.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
				return;
			}

            switch ( info.ButtonID )
            {
                case (int)Buttons.Okay:
					
                    Item item = null;

                    foreach (Type type in this.m_Selected)
                    {
                        try
                        {
                            item = Activator.CreateInstance(type) as Item;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }

                        if (item != null)
                        {
                            this.m_Token.Delete();
                            sender.Mobile.AddToBackpack(item);
                        }
                    }

                    break;
                case (int)Buttons.Cancel:
                    sender.Mobile.SendGump(new HeritageTokenGump(this.m_Token, this.m_User));
                    break;
            }
        }
    }
}