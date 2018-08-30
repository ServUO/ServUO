using System;

using Server;
using Server.Gumps;

namespace Arya.Chess
{
	public class PawnPromotionGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;

		ChessGame m_Game;

		public PawnPromotionGump( Mobile m, ChessGame game ) : base( 200, 200 )
		{
			m.CloseGump( typeof( PawnPromotionGump ) );
			m_Game = game;
			MakeGump();
		}

		private void MakeGump()
		{
			this.Closable=false;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBackground(0, 0, 255, 185, 9200);
			this.AddImageTiled(0, 0, 255, 185, 9304);
			this.AddImageTiled(1, 1, 253, 183, 9274);
			this.AddAlphaRegion(1, 1, 253, 183);

			this.AddLabel(20, 10, GreenHue, @"Your pawn is being promoted!");

			this.AddLabel(20, 35, LabelHue, @"Promote to:");

			this.AddButton(35, 70, 2337, 2344, 1, GumpButtonType.Reply, 0);
			this.AddLabel(25, 120, LabelHue, @"Queen");

			this.AddButton(85, 70, 2333, 2340, 2, GumpButtonType.Reply, 0);
			this.AddLabel(80, 120, LabelHue, @"Rook");

			this.AddButton(135, 70, 2335, 2344, 3, GumpButtonType.Reply, 0);
			this.AddLabel(130, 120, LabelHue, @"Knight");

			this.AddButton(195, 70, 2332, 2339, 4, GumpButtonType.Reply, 0);
			this.AddLabel(185, 120, LabelHue, @"Bishop");
			
			this.AddButton(25, 152, 9702, 248, 5, GumpButtonType.Reply, 0);
			this.AddLabel(50, 150, 0, @"Do not promote pawn");
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			PawnPromotion type = PawnPromotion.None;

			switch ( info.ButtonID )
			{
				case 0 :
						return; // This fixes a crash when staff deletes the pawn being promoted

				case 1 : type = PawnPromotion.Queen;
					break;

				case 2 : type = PawnPromotion.Rook;
					break;

				case 3 : type = PawnPromotion.Knight;
					break;

				case 4 : type = PawnPromotion.Bishop;
					break;

				case 5: type = PawnPromotion.None;
					break;
			}

			m_Game.OnPawnPromoted( type );
		}
	}
}
