using System;
using Server;

namespace Knives.Chat3
{
	public class ErrorsNotifyGump : GumpPlus
	{
		public ErrorsNotifyGump( Mobile m ) : base( m, 200, 100 )
		{
			m.CloseGump( typeof( ErrorsNotifyGump ) );
		}

		protected override void BuildGump()
		{
			AddButton( 0, 0, 0xDF, 0xDF, "Errors", new GumpCallback( Errors ) );
		}

		private void Errors()
		{
			new ErrorsGump( Owner );
		}
	}
}