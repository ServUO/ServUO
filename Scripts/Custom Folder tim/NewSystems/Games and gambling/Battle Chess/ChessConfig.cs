using System;

namespace Arya.Chess
{
	public class ChessConfig
	{
		/// <summary>
		/// The time out for the game to start. If a second player hasn't accepted the game within this time
		/// the game will be reset.
		/// </summary>
		public static TimeSpan GameStartTimeOut = TimeSpan.FromMinutes( 2 );
		/// <summary>
		/// The maximum time allowed for a player to make a move. If no move is made within this amount of time
		/// the game will be reset.
		/// </summary>
		public static TimeSpan MoveTimeOut = TimeSpan.FromMinutes( 10 );
		/// <summary>
		/// When a player disconnects, the game will give them time to get back to their game (to handle
		/// player system crashes, connections faults and so on). If one of the player doesn't log back in within
		/// this time frame, the game is reset.
		/// </summary>
		public static TimeSpan DisconnectTimeOut = TimeSpan.FromMinutes( 10 );
		/// <summary>
		/// This is the amount of time given to players before the game ends after they have been notified of the
		/// move time out. Also when the game ends regularly, both players get a gump asking to confirm the end
		/// of the game. If they don't close it within this time frame, the game will reset.
		/// </summary>
		public static TimeSpan EndGameTimerOut = TimeSpan.FromMinutes( 3 );
		/// <summary>
		/// Specifies whether the winner should receive a reward scroll or not after the game is over.
		/// No scroll is given for stalemate, or canceled games. This item has no function in the real world,
		/// its properties only show the results of the game.
		/// </summary>
		public static bool GiveRewardScroll = true;
		/// <summary>
		/// This is the keyword that can be used to restore the gumps if anything goes wrong and the gump disappears
		/// </summary>
		public static string ResetKeyword = "game";
	}
}
