using System;
using Server;

namespace Knives.Chat3
{
	public class PmNotifyGump20 : GumpPlus
	{
		public PmNotifyGump20( Mobile m ) : base( m, 200, 50 )
		{
			m.CloseGump( typeof( PmNotifyGump20 ) );
		}

		protected override void BuildGump()
		{
            if (Data.GetData(Owner).GetMsg() != null)
            {
                AddButton(30, 10, 0x82E, "Message", new GumpCallback(Message));
                AddImage(0, 0, 0x9CB);
                AddImageTiled(35, 7, 20, 8, 0x9DC);
                AddHtml(23, 1, 50, "<CENTER>" + HTML.Black + Data.GetData(Owner).GetMsg().From.RawName);
            }
        }

        private void Message()
        {
            Data.GetData(Owner).CheckMsg();

            if (Data.GetData(Owner).NewMsg())
                NewGump();
        }
    }
}