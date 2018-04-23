using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class RiddleTeleporter : Teleporter
	{
		private string m_CurrentQ;
		private string m_CurrentA;

		private TimeSpan m_QChangeTime;

		private string m_Question;
		private string m_Answer;

		private string m_AnswerA;
		private string m_AnswerB;
		private string m_AnswerC;
		private string m_AnswerD;
		private string m_AnswerE;
		private string m_AnswerF;

		private string m_QuestionA;
		private string m_QuestionB;
		private string m_QuestionC;
		private string m_QuestionD;
		private string m_QuestionE;
		private string m_QuestionF;

		private int m_Range;

		private bool m_AllowGhost;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowGhosts
		{
			get{ return m_AllowGhost; }
			set{ m_AllowGhost = value; }
		}

		[CommandProperty( AccessLevel.Seer )]
		public string CurrentQ
		{
			get{ return m_CurrentQ; }
			set{ m_CurrentQ = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.Seer )]
		public string CurrentA
		{
			get{ return m_CurrentA; }
			set{ m_CurrentA = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan QChangeTime
		{
			get{ return m_QChangeTime; }
			set{ m_QChangeTime = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string AnswerA
		{
			get{ return m_AnswerA; }
			set{ if ( m_QuestionA != null ) m_AnswerA = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string AnswerB
		{
			get{ return m_AnswerB; }
			set{ if ( m_QuestionB != null ) m_AnswerB = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string AnswerC
		{
			get{ return m_AnswerC; }
			set{ if ( m_QuestionC != null ) m_AnswerC = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string AnswerD
		{
			get{ return m_AnswerD; }
			set{ if ( m_QuestionD != null ) m_AnswerD = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string AnswerE
		{
			get{ return m_AnswerE; }
			set{ if ( m_QuestionE != null ) m_AnswerE = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string AnswerF
		{
			get{ return m_AnswerF; }
			set{ if ( m_QuestionF != null ) m_AnswerF = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestionA
		{
			get{ return m_QuestionA; }
			set{ m_QuestionA = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestionB
		{
			get{ return m_QuestionB; }
			set{ if ( m_QuestionA != null && m_AnswerA != null ) m_QuestionB = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestionC
		{
			get{ return m_QuestionC; }
			set{ if ( m_QuestionB != null && m_AnswerB != null ) m_QuestionC = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestionD
		{
			get{ return m_QuestionD; }
			set{ if ( m_QuestionC != null && m_AnswerC != null ) m_QuestionD = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestionE
		{
			get{ return m_QuestionE; }
			set{ if ( m_QuestionD != null && m_AnswerD != null ) m_QuestionE = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string QuestionF
		{
			get{ return m_QuestionF; }
			set{ if ( m_QuestionE != null && m_AnswerE != null ) m_QuestionF = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range
		{
			get{ return m_Range; }
			set{ m_Range = value; InvalidateProperties(); }
		}

		public override bool HandlesOnSpeech{ get{ return true; } }

		private void TellQuestion()
		{
			ArrayList list = new ArrayList();

			foreach (Mobile m in this.GetMobilesInRange(m_Range))
			{
				if (!m.Alive && m_AllowGhost == false)
					return;

				if (m.Player && m.AccessLevel == AccessLevel.Player)
					list.Add(m);
			}

			foreach (Mobile m in list)
			{
				m.SendMessage("The question is currently:");
				m.SendMessage("{0}", m_Question);
			}
		}

		private void ChooseQuestion()
		{
			if ( m_QuestionA != null && m_QuestionB == null && m_QuestionC == null && m_QuestionD == null && m_QuestionE == null && m_QuestionF == null)
			{
				m_Question = m_QuestionA;
				m_CurrentQ = m_QuestionA;
			}

			if ( m_QuestionB != null && m_QuestionC == null && m_QuestionD == null && m_QuestionE == null && m_QuestionF == null)
			{
				String[] st = {m_QuestionA, m_QuestionB};
				String question = (st[Utility.Random(2)]);
				m_Question = question;
				m_CurrentQ = question;
			}

			if ( m_QuestionC != null && m_QuestionD == null && m_QuestionE == null && m_QuestionF == null)
			{
				String[] st = {m_QuestionA, m_QuestionB, m_QuestionC};
				String question = (st[Utility.Random(3)]);
				m_Question = question;
				m_CurrentQ = question;
			}

			if ( m_QuestionD != null && m_QuestionE == null && m_QuestionF == null)
			{
				String[] st = {m_QuestionA, m_QuestionB, m_QuestionC, m_QuestionD};
				String question = (st[Utility.Random(4)]);
				m_Question = question;
				m_CurrentQ = question;
			}

			if ( m_QuestionE != null && m_QuestionF == null)
			{
				String[] st = {m_QuestionA, m_QuestionB, m_QuestionC, m_QuestionD, m_QuestionE};
				String question = (st[Utility.Random(5)]);
				m_Question = question;
				m_CurrentQ = question;
			}

			if ( m_QuestionF != null )
			{
				String[] st = {m_QuestionA, m_QuestionB, m_QuestionC, m_QuestionD, m_QuestionE, m_QuestionF};
				String question = (st[Utility.Random(6)]);
				m_Question = question;
				m_CurrentQ = question;
			}

			ChooseAnswer();
		}

		private void ChooseAnswer()
		{
			if ( m_Question == m_QuestionA )
			{
				m_Answer = m_AnswerA;
				m_CurrentA = m_AnswerA;
			}

			if ( m_Question == m_QuestionB )
			{
				m_Answer = m_AnswerB;
				m_CurrentA = m_AnswerB;
			}

			if ( m_Question == m_QuestionC )
			{
				m_Answer = m_AnswerC;
				m_CurrentA = m_AnswerC;
			}

			if ( m_Question == m_QuestionD )
			{
				m_Answer = m_AnswerD;
				m_CurrentA = m_AnswerD;
			}

			if ( m_Question == m_QuestionE )
			{
				m_Answer = m_AnswerE;
				m_CurrentA = m_AnswerE;
			}

			if ( m_Question == m_QuestionF )
			{
				m_Answer = m_AnswerF;
				m_CurrentA = m_AnswerF;
			}
		}

		private DateTime m_NextQuestion;

		public override bool HandlesOnMovement{ get{ return Active; } }

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( !m.Alive && m_AllowGhost == false )
				return;

			if ( Active != false && Utility.InRange( m.Location, this.Location, m_Range ) && !Utility.InRange( oldLocation, this.Location, m_Range ) )
			{
				TellQuestion();

				if ( DateTime.Now >= m_NextQuestion )
				{
					m_NextQuestion = DateTime.Now + m_QChangeTime;
					ChooseQuestion();
				}
			}
			base.OnMovement( m, oldLocation );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			Mobile m = e.Mobile;

			if ( !m.Alive && m_AllowGhost == false )
				return;

			if ( !e.Handled && Active )
			{
				if ( !Creatures && !m.Player)
					return;

				if ( !m.InRange( GetWorldLocation(), m_Range ) )
					return;

				bool isMatch = false;

				if ( m_Answer != null && e.Speech.ToLower().IndexOf( m_Answer.ToLower() ) >= 0 )
					isMatch = true;

                if ( !isMatch )
					return;

				e.Handled = true;
				StartTeleport( m );
				m.SendMessage( "Well done {0} you may proceed", m.Name );
			}
		}

		public override bool OnMoveOver( Mobile m )
		{
			return true;
		}

		[Constructable]
		public RiddleTeleporter()
		{
			Active = false;
			m_AllowGhost = false;

			m_CurrentA = null;
			m_CurrentQ = null;

			m_Question = null;
			m_Answer= null;

			m_AnswerA = null;
			m_AnswerB = null;
			m_AnswerC = null;
			m_AnswerD = null;
			m_AnswerE = null;
			m_AnswerF = null;

			m_QuestionA = null;
			m_QuestionB = null;
			m_QuestionC = null;
			m_QuestionD = null;
			m_QuestionE = null;
			m_QuestionF = null;

			ChooseQuestion();
		}

		public RiddleTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (TimeSpan) m_QChangeTime );

			writer.Write( m_Question );
			writer.Write( m_Answer);

			writer.Write( m_AnswerA );
			writer.Write( m_AnswerB );
			writer.Write( m_AnswerC );
			writer.Write( m_AnswerD );
			writer.Write( m_AnswerE );
			writer.Write( m_AnswerF );

			writer.Write( m_QuestionA );
			writer.Write( m_QuestionB );
			writer.Write( m_QuestionC );
			writer.Write( m_QuestionD );
			writer.Write( m_QuestionE );
			writer.Write( m_QuestionF );
			writer.Write( m_Range );
			writer.Write( m_AllowGhost );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_QChangeTime = reader.ReadTimeSpan();

					m_Question = reader.ReadString();
					m_Answer = reader.ReadString();

					m_AnswerA = reader.ReadString();
					m_AnswerB = reader.ReadString();
					m_AnswerC = reader.ReadString();
					m_AnswerD = reader.ReadString();
					m_AnswerE = reader.ReadString();
					m_AnswerF = reader.ReadString();

					m_QuestionA = reader.ReadString();
					m_QuestionB = reader.ReadString();
					m_QuestionC = reader.ReadString();
					m_QuestionD = reader.ReadString();
					m_QuestionE = reader.ReadString();
					m_QuestionF = reader.ReadString();
					m_Range = reader.ReadInt();
					m_AllowGhost = reader.ReadBool();

					break;
				}
			}
		}
	}
}