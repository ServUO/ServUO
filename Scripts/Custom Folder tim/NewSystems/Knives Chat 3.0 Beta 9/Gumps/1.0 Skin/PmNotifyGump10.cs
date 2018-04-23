using System;
using Server;

namespace Knives.Chat3
{
	public class PmNotifyGump10 : GumpPlus
	{
		public PmNotifyGump10( Mobile m ) : base( m, 200, 50 )
		{
			m.CloseGump( typeof( PmNotifyGump10 ) );
		}

		protected override void BuildGump()
		{
            if (Data.GetData(Owner).GetMsg() != null)
            {
                int offset = 30;
                int width = 120;

                AddBackground(offset, 0, width, 60, Data.GetData(Owner).DefaultBack);

                AddButton(0, 15, 0x2634, 0x2635, "Message", new GumpCallback(Message));
                AddHtml(offset, 8, width, 45, "<CENTER>Message from<BR>" + Data.GetData(Owner).GetMsg().From.RawName, false, false);
                AddHtml(5, 17, 25, "<CENTER>" + Data.GetData(Owner).Messages.Count);
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