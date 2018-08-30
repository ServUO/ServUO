#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;

using Server;
using Server.Targeting;

namespace Arya.Abay
{
	/// <summary>
	/// General purpose target used by the Abay system
	/// </summary>
	public class AbayTarget : Target
	{
		private AbayTargetCallback m_Callback;

		public AbayTarget( AbayTargetCallback callback, int range, bool allowground ) : base( range, allowground, TargetFlags.None )
		{
			m_Callback = callback;
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			try
			{
				m_Callback.DynamicInvoke( new object[] { from, targeted } );
			}
			catch
			{
				Console.WriteLine( "The Abay system cannot access the cliloc.enu file. Please review the system instructions for proper installation" );
			}
		}

		protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
		{
			if ( AbaySystem.Running )
			{
				from.SendGump( new AbayGump( from ) );
			}
		}
	}
}
