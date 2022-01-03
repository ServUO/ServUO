using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlMorph : XmlAttachment
	{
		private string m_Word = null;     // no word activation by default
		private int m_OriginalID = -1;                  // default value indicating that it has not been morphed
		private int m_MorphID;
		private int proximityrange = 2;                 // default movement activation from 5 tiles away
		private TimeSpan m_Duration = TimeSpan.FromSeconds(30.0);    // default 30 second duration
		private MorphTimer m_MorphTimer;
		private DateTime m_MorphEnd;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MorphID { get { return m_MorphID; } set { m_MorphID  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Duration { get { return m_Duration; } set { m_Duration  = value; } }
        
		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime MorphEnd { get { return m_MorphEnd; }  }

		[CommandProperty( AccessLevel.GameMaster )]
		public string ActivationWord { get { return m_Word; } set { m_Word  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range { get { return proximityrange; } set { proximityrange  = value; } }

		// These are the various ways in which the message attachment can be constructed.
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlMorph(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlMorph(int morphID)
		{
			m_MorphID = morphID;
		}
        
		[Attachable]
		public XmlMorph(int morphID, double duration)
		{
			m_MorphID = morphID;
			m_Duration = TimeSpan.FromMinutes(duration);
		}

		[Attachable]
		public XmlMorph(int morphID, double duration, string word)
		{
			m_MorphID = morphID;
			m_Duration = TimeSpan.FromMinutes(duration);
			ActivationWord = word;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 1 );
			// version 1
			writer.Write(proximityrange);
			// version 0
			writer.Write(m_OriginalID);
			writer.Write(m_MorphID);
			writer.Write(m_Duration);
			writer.Write(m_Word);
			if(m_MorphTimer != null)
			{
				writer.Write(m_MorphEnd - DateTime.Now);
			} 
			else
			{
				writer.Write(TimeSpan.Zero);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch(version)
			{
				case 1:
					Range = reader.ReadInt();
					goto case 0;
				case 0:
					// version 0
    
					m_OriginalID = reader.ReadInt();
					m_MorphID = reader.ReadInt();
					m_Duration = reader.ReadTimeSpan();
					ActivationWord = reader.ReadString();
					TimeSpan remaining = (TimeSpan)reader.ReadTimeSpan();
    
					if(remaining > TimeSpan.Zero)
						DoTimer(remaining);
					break;
			}
		}

		public override string OnIdentify(Mobile from)
		{
			base.OnIdentify(from);

			if(from == null || from.AccessLevel == AccessLevel.Player) return null;

			string msg = null;

			if(Expiration > TimeSpan.Zero)
			{
				msg = String.Format("Morph to {0} expires in {1} mins",m_MorphID,Expiration.TotalMinutes);
			} 
			else
			{
				msg = String.Format("Morph to {0} duration {1} mins",m_MorphID, m_Duration.TotalMinutes);
			}

			if(ActivationWord != null)
			{
				return String.Format("{0} activated by '{1}'",msg, ActivationWord);
			} 
			else
			{
				return msg;
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			// remove the mod
			if(AttachedTo is Mobile)
			{
				((Mobile)AttachedTo).BodyMod = m_OriginalID;
			} 
			else
				if(AttachedTo is Item)
			{
				((Item)AttachedTo).ItemID = m_OriginalID;
			}
		}
        
		public override bool HandlesOnSpeech { get { return (ActivationWord != null); } }

		public override void OnSpeech(SpeechEventArgs e )
		{
			base.OnSpeech(e);
		    
			if(e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player) return;
		    
			// dont respond to other players speech if this is attached to a mob
			if(AttachedTo is Mobile && (Mobile)AttachedTo != e.Mobile) return;

			if(e.Speech == ActivationWord)
			{
				OnTrigger(null, e.Mobile);
			}
		}

		public override bool HandlesOnMovement { get { return (ActivationWord == null); } }

		public override void OnMovement(MovementEventArgs e )
		{
			base.OnMovement(e);
		    
			if(e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player) return;

			if(AttachedTo is Item && (((Item)AttachedTo).Parent == null) && Utility.InRange( e.Mobile.Location, ((Item)AttachedTo).Location, proximityrange ))
			{
				OnTrigger(null, e.Mobile);
			} 
			else
				return;
		}

		public override void OnAttach()
		{
			base.OnAttach();

			// apply the mod immediately if attached to a mob
			if(AttachedTo is Mobile)
			{
				Mobile m = AttachedTo as Mobile;
				m_OriginalID = m.BodyMod;
				m.BodyMod = m_MorphID;
				Expiration = m_Duration;
			}
		}

		public override void OnReattach()
		{
			base.OnReattach();

			// reapply the mod if attached to a mob
			if(AttachedTo is Mobile)
			{
				((Mobile)AttachedTo).BodyMod = m_MorphID;
			}
		}

		// ----------------------------------------------
		// Private methods
		// ----------------------------------------------
		private void DoTimer(TimeSpan delay)
		{
			m_MorphEnd = DateTime.Now + delay;

			if ( m_MorphTimer != null )
				m_MorphTimer.Stop();

			m_MorphTimer = new MorphTimer( this, delay);
			m_MorphTimer.Start();
		}
            
		// a timer that can be implement limited lifetime morph
		private class MorphTimer : Timer
		{
			private XmlMorph m_Attachment;

			public MorphTimer( XmlMorph attachment, TimeSpan delay) : base( delay )
			{
				Priority = TimerPriority.OneSecond;

				m_Attachment = attachment;
			}

			protected override void OnTick()
			{
				if(m_Attachment != null && !m_Attachment.Deleted && m_Attachment.AttachedTo is Item && !((Item)m_Attachment.AttachedTo).Deleted)
				{
					Item i = m_Attachment.AttachedTo as Item;
					i.ItemID = m_Attachment.m_OriginalID;
					m_Attachment.m_OriginalID = -1;
				}
			}
		}

		public override void OnTrigger(object activator, Mobile m)
		{
			if(m == null ) return;
            
			// if attached to an item then morph and then reset after duration
			// note that OriginalID will be -1 if the target is not already morphed
			if(AttachedTo is Item && m_OriginalID == -1)
			{
				Item i = AttachedTo as Item;
				m_OriginalID = i.ItemID;
				i.ItemID = m_MorphID;

				// start the timer to reset the ID
				DoTimer(m_Duration);
			}
		}

	}
}
