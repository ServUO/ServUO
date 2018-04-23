using System;

using Server;

namespace Arya.Chess
{
	/// <summary>
	/// Describes a piece move
	/// </summary>
	public class Move
	{
		private Point2D m_From;
		private Point2D m_To;
		private BaseChessPiece m_Piece;
		private BaseChessPiece m_Captured;
		private bool m_EnPassant = false;

		/// <summary>
		/// Gets the initial position of the piece
		/// </summary>
		public Point2D From
		{
			get { return m_From; }
		}

		/// <summary>
		/// Gets the target destination of the move
		/// </summary>
		public Point2D To
		{
			get { return m_To; }
		}

		/// <summary>
		/// Gets the chess piece performing this move
		/// </summary>
		public BaseChessPiece Piece
		{
			get { return m_Piece; }
		}

		/// <summary>
		/// Gets the piece captured by this move
		/// </summary>
		public BaseChessPiece CapturedPiece
		{
			get { return m_Captured; }
		}

		/// <summary>
		/// Specifies if this move captures a piece
		/// </summary>
		public bool Capture
		{
			get { return m_Captured != null; }
		}

		/// <summary>
		/// The color of the player making this move
		/// </summary>
		public ChessColor Color
		{
			get { return m_Piece.Color; }
		}

		/// <summary>
		/// Gets the color of the opponent of the player who made the move
		/// </summary>
		public ChessColor EnemyColor
		{
			get { return m_Piece.EnemyColor; }
		}

		/// <summary>
		/// Specifies if the capture is made EnPassant
		/// </summary>
		public bool EnPassant
		{
			get { return m_EnPassant; }
			set { m_EnPassant = value; }
		}

		/// <summary>
		/// Creates a new Move object without capturing a piece
		/// </summary>
		/// <param name="piece">The chess piece performing the move</param>
		/// <param name="target">The target location of the move</param>
		public Move( BaseChessPiece piece, Point2D target )
		{
			m_Piece = piece;
			m_From = m_Piece.Position;
			m_To = target;
			m_Captured = m_Piece.GetCaptured( target, ref m_EnPassant );
		}
	}
}