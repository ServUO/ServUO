using System;
using Server;

namespace Knives.Chat3
{
	public class PmNotifyGump : GumpPlus
	{
		public PmNotifyGump( Mobile m ) : base( m, 200, 50 )
		{
			m.CloseGump( typeof( PmNotifyGump ) );
		}

		protected override void BuildGump()
		{
            AddImage(0, 0, 0x15D5);
            AddButton(13, 13, 0xFC4, "Message", new GumpCallback(Message));
		}

		private void Message()
		{
            General.List(Owner, 2);
		}
	}
}