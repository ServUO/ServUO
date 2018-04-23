using System;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Gumps
{
	public class MinerPayTableGump : Gump
	{
		private MinerSlotStone m_Stone;

		public MinerPayTableGump( MinerSlotStone stone ) : base( 25, 25 )
		{
			m_Stone = stone;

			int p1 = stone.Cost * 10000;
			int p2 = stone.Cost * 5000;
			int p3 = stone.Cost * 1000;
			int p4 = stone.Cost * 500;
			int p5 = stone.Cost * 10;
			int p6 = stone.Cost * 5;
			int p7 = stone.Cost * 3;
			int p8 = stone.Cost * 2;

			int p9 = stone.Cost / 2;
			int p10 = stone.Cost / 3;

			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;
			AddPage(0);
			AddBackground(37, 34, 287, 459, 5120);
			AddImageTiled(45, 351, 101, 64, 2524);
			AddImageTiled(45, 84, 101, 235, 2524);
			AddLabel(45, 64, 1160, @"Pay Table");
			AddLabel(45, 330, 1160, @"Scatter Pay");
			AddItem(45, 85, 7147);
			AddItem(45, 115, 7159);
			AddItem(45, 145, 7141);
			AddItem(45, 175, 7153);
			AddItem(45, 205, 7147);
			AddItem(45, 235, 4020);
			AddItem(75, 85, 7147);
			AddItem(105, 85, 7147);
			AddItem(75, 115, 7159);
			AddItem(105, 115, 7159);
			AddItem(75, 145, 7141);
			AddItem(105, 145, 7141);
			AddItem(75, 175, 7153);
			AddItem(105, 175, 7153);
			AddItem(105, 205, 7153);
			AddLabel(61, 208, 1149, @"Any 3 Bars");
			AddItem(75, 235, 4020);
			AddItem(105, 235, 4020);
			AddItem(45, 265, 5091);
			AddItem(45, 295, 6262);
			AddItem(75, 265, 5091);
			AddItem(105, 265, 5091);
			AddItem(75, 295, 6262);
			AddItem(105, 295, 6262);
			AddItem(45, 355, 3717);
			AddItem(45, 385, 3717);
			AddLabel(45, 426, 1160, @"Bonus Round");
			AddItem(75, 385, 3717);
			AddLabel(80, 353, 1149, @"Any One");
			AddLabel(80, 383, 1149, @"Any Two");
			AddLabel(155, 85, 1160, p1.ToString() );
			AddLabel(155, 115, 1160, p2.ToString() );
			AddLabel(155, 145, 1160, p3.ToString() );
			AddLabel(155, 175, 1160, p4.ToString() );
			AddLabel(155, 205, 1160, p5.ToString() );
			AddLabel(155, 235, 1160, p6.ToString() );
			AddLabel(155, 265, 1160, p7.ToString() );
			AddHtml( 45, 41, 269, 22, @"<BASEFONT COLOR=#FFFFFF><CENTER>Miner Madness</CENTER></BASEFONT>", (bool)false, (bool)false);
			AddLabel(155, 295, 1160, p8.ToString() );
			AddLabel(155, 355, 1160, p10.ToString() );
			AddLabel(155, 385, 1160, p9.ToString() );
			AddImageTiled(44, 448, 101, 35, 2524);
			AddItem(45, 452, 3717);
			AddItem(75, 452, 3717);
			AddItem(105, 452, 3717);
			AddLabel(155, 452, 1160, @"Mining Bonus Round");

		}
	}
}