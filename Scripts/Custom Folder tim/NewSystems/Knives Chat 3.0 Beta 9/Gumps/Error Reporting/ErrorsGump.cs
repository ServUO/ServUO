using System;
using Server;

namespace Knives.Chat3
{
	public class ErrorsGump : GumpPlus
	{
		public ErrorsGump( Mobile m ) : base( m, 100, 100 )
		{
            Errors.Checked.Add(m);

			m.CloseGump( typeof( ErrorsGump ) );
		}

		protected override void BuildGump()
		{
            int width = 400;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(201));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddButton(width - 20, y, 0x5689, "Help", new GumpCallback(Help));

            string str = HTML.Black;
			foreach( string text in Errors.ErrorLog )
				str += text;

			AddHtml( 20, y+=25, width-40, 200, str, true, true );

            y += 200;

			if ( Owner.AccessLevel >= AccessLevel.Administrator )
			{
				AddButton( width/2-30, y+=10, 0x98B, 0x98B, "Clear", new GumpCallback( ClearLog ) );
				AddHtml( width/2-23, y+3, 51, "<CENTER>" + General.Local(202));
			}

            AddBackgroundZero(0, 0, width, y + 40, 0x1400);
        }

		private void Help()
		{
			NewGump();
			new Chat3.InfoGump( Owner, 300, 300, HTML.White +"     " + General.Local(200), true );
		}

		private void ClearLog()
		{
			Errors.ErrorLog.Clear();
            NewGump();
		}
	}
}