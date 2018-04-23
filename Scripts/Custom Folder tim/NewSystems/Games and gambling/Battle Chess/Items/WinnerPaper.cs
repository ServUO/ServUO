using System;

using Server;
using Server.Items;

namespace Arya.Chess
{
	[ Flipable( 5357, 5358 ) ]
	public class WinnerPaper : Item
	{
		private string m_Winner;
		private string m_Looser;
		private DateTime m_GameEnd;
		private TimeSpan m_GameTime;
		private TimeSpan m_WinnerTime;
		private TimeSpan m_LooserTime;
		private int m_WinnerScore;
		private int m_LooserScore;

		public WinnerPaper( Mobile winner, Mobile looser, TimeSpan gameTime, TimeSpan winnerTime, TimeSpan looserTime, int winnerScore, int looserScore) : base( 5357 )
		{
			Hue = 1159;
			Weight = 0.2;

			m_Winner = winner.Name;
			m_Looser = looser.Name;
			m_GameEnd = DateTime.Now;
			m_GameTime = gameTime;
			m_WinnerTime = winnerTime;
			m_LooserTime = looserTime;
			m_WinnerScore = winnerScore;
			m_LooserScore = looserScore;

			Name = string.Format( "{0} won a fair chess game vs {1}", m_Winner, m_Looser );
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties (list);

			list.Add( 1060658, "Date\t{0}", m_GameEnd.ToLongDateString() );
			list.Add( 1060659, "Total Game Time\t{0} Hours, {1} Minutes, {2} Seconds", m_GameTime.Hours, m_GameTime.Minutes, m_GameTime.Seconds );
			list.Add( 1060660, "{0}'s Game Time\t{1} Hours, {2} Minutes, {3} Seconds", m_Winner, m_WinnerTime.Hours, m_WinnerTime.Minutes, m_WinnerTime.Seconds );
			list.Add( 1060661, "{0}'s Game Time\t{1} Hours, {2} Minutes, {3} Seconds", m_Looser, m_LooserTime.Hours, m_LooserTime.Minutes, m_LooserTime.Seconds );
			list.Add( 1060662, "{0}'s Score\t{1}", m_Winner, m_WinnerScore );
			list.Add( 1060663, "{0}'s Score\t{1}", m_Looser, m_LooserScore );
		}


		public WinnerPaper( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();

			m_Winner = reader.ReadString();
			m_Looser = reader.ReadString();
			m_GameEnd = reader.ReadDateTime();
			m_GameTime = reader.ReadTimeSpan();
			m_WinnerTime = reader.ReadTimeSpan();
			m_LooserTime = reader.ReadTimeSpan();
			m_WinnerScore = reader.ReadInt();
			m_LooserScore = reader.ReadInt();
		}


		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( 0 );

			writer.Write( m_Winner );
			writer.Write( m_Looser );
			writer.Write( m_GameEnd );
			writer.Write( m_GameTime	);
			writer.Write( m_WinnerTime );
			writer.Write( m_LooserTime );
			writer.Write( m_WinnerScore );
			writer.Write( m_LooserScore );
		}
	}
}
