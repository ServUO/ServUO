#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;

using Server;

namespace Arya.Abay
{
	/// <summary>
	/// Summary description for AbayScheduler.
	/// </summary>
	public class AbayScheduler
	{
		private static InternalTimer m_Timer;
		private static DateTime m_Deadline = DateTime.MaxValue;

		/// <summary>
		/// Gets the next deadline
		/// </summary>
		public static DateTime Deadline
		{
			get
			{
				return m_Deadline;
			}
		}

		public static void Initialize()
		{
			m_Timer = new InternalTimer();
			ResetTimer();
		}

		public static void Stop()
		{
			if ( m_Timer != null )
			{
				m_Timer.Stop();
				m_Timer = null;
			}
		}

		private static void ResetTimer()
		{
			if ( AbaySystem.Running )
			{
				CalculateDeadline();
			}
			m_Timer.Start();
		}

		/// <summary>
		/// Calculates the next deadline for the scheduler
		/// </summary>
		private static void CalculateDeadline()
		{
			ArrayList list = new ArrayList( AbaySystem.Abays );
			list.AddRange( AbaySystem.Pending );

			m_Deadline = DateTime.MaxValue;

			foreach( AbayItem Abay in list )
			{
				if ( Abay.Deadline < m_Deadline )
				{
					m_Deadline = Abay.Deadline;
				}
			}
		}

		/// <summary>
		/// This method accepts a new deadline being added to the system
		/// </summary>
		/// <param name="deadline">The new deadline</param>
		public static void UpdateDeadline( DateTime deadline )
		{
			if ( deadline < m_Deadline )
			{
				m_Deadline = deadline;
			}
		}

		/// <summary>
		/// Fires the DeadlineReached event
		/// </summary>
		private static void OnDeadlineReached()
		{
			AbaySystem.OnDeadlineReached();
		}

		private static void OnTimer()
		{
			if ( m_Deadline < DateTime.Now )
			{
				m_Timer.Stop();

				OnDeadlineReached();

				ResetTimer();
			}
		}

		private class InternalTimer : Timer
		{
			public InternalTimer() : base( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 5.0 ) )
			{
			}

			protected override void OnTick()
			{
				AbayScheduler.OnTimer();
			}
		}
	}
}