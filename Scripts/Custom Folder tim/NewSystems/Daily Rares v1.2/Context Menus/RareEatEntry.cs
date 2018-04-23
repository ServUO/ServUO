using System;
using Server.Items;

namespace Server.ContextMenus
{
	public class RareEatEntry : ContextMenuEntry
	{
		private Mobile m_From;
		private BaseDailyRareFood m_Food;

		public RareEatEntry( Mobile from, BaseDailyRareFood food ) : base( 6135, 1 )
		{
			m_From = from;
			m_Food = food;
		}

		public override void OnClick()
		{
			if ( m_Food.Deleted || !m_Food.Movable || !m_From.CheckAlive() )
				return;

			m_Food.Eat( m_From );
		}
	}
}