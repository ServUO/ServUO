using System;

using Server;
using Server.Targeting;

namespace Arya.Chess
{
	public delegate void ChessTargetCallback( Mobile from, object targeted );

	/// <summary>
	/// General purpose target for the chess system
	/// </summary>
	public class ChessTarget : Target
	{
		/// <summary>
		/// The message for the target request
		/// </summary>
		private string m_Message;
		/// <summary>
		/// The callback for this target
		/// </summary>
		private ChessTargetCallback m_Callback;
		/// <summary>
		/// The chess game managing this target
		/// </summary>
		private ChessGame m_Game;
		/// <summary>
		/// Flag for a target used outside a game
		/// </summary>
		private bool m_IgnoreGame = false;
		
		public ChessTarget( ChessGame game, Mobile m, string message, ChessTargetCallback callback ) : base( -1, true, TargetFlags.None )
		{
			m_Message = message;
			m_Callback = callback;
			m_Game = game;

			if ( message != null )
				m.SendMessage( 0x40, message );
		}

		public ChessTarget( Mobile m, string message, ChessTargetCallback callback ) : base( -1, true, TargetFlags.None )
		{
			m_IgnoreGame = true;
			m_Message = message;
			m_Callback = callback;

			if ( message != null )
				m.SendMessage( 0x40, message );
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			if ( !m_IgnoreGame && ( m_Game == null || !m_Game.AllowTarget ) )
				return;

			if ( m_Callback != null )
			{
				try
				{
					m_Callback.DynamicInvoke( new object[] { from, targeted } );
				}
				catch ( Exception err )
				{
					Console.WriteLine( err.ToString() );
				}
			}
		}

		public void Remove( Mobile m )
		{
			Invoke( m, new Point3D( 0, 0, 0 ) );
		}
	}
}