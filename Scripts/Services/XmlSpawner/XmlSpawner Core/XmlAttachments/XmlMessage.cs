using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlMessage : XmlAttachment
	{
		private string m_MessageStr;
		private string m_Word = null;     // no word activation by default
		private TimeSpan m_Refractory = TimeSpan.FromSeconds(5);    // 5 seconds default time between activations
		private DateTime m_EndTime;
		private int m_Charges = 0;                        // no charge limit
		private int proximityrange = 5;                 // default movement activation from 5 tiles away

		[CommandProperty( AccessLevel.GameMaster )]
		public string Message { get { return m_MessageStr; } set { m_MessageStr  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range { get { return proximityrange; } set { proximityrange  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string ActivationWord { get { return m_Word; } set { m_Word  = value; } }
        
		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges { get { return m_Charges; } set { m_Charges  = value; } }
        
		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Refractory { get { return m_Refractory; } set { m_Refractory  = value; } }

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlMessage(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlMessage(string msg)
		{
			Message = msg;
		}

		[Attachable]
		public XmlMessage(string msg, double refractory)
		{
			Message = msg;
			Refractory = TimeSpan.FromSeconds(refractory);
		}
        
		[Attachable]
		public XmlMessage(string msg, double refractory, string word )
		{
			ActivationWord = word;
			Message = msg;
			Refractory = TimeSpan.FromSeconds(refractory);
		}
        
		[Attachable]
		public XmlMessage(string msg, double refractory, string word, int charges )
		{
			ActivationWord = word;
			Message = msg;
			Refractory = TimeSpan.FromSeconds(refractory);
			Charges = charges;

		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 1 );
			// version 1
			writer.Write(proximityrange);
			// version 0
			writer.Write(m_MessageStr);
			writer.Write(m_Word);
			writer.Write(m_Charges);
			writer.Write(m_Refractory);
			writer.Write(m_EndTime - DateTime.Now);
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
					Message = reader.ReadString();
					ActivationWord = reader.ReadString();
					Charges = reader.ReadInt();
					Refractory = reader.ReadTimeSpan();
					TimeSpan remaining = reader.ReadTimeSpan();
					m_EndTime = DateTime.Now + remaining;
					break;
			}
		}

		public override string OnIdentify(Mobile from)
		{
			if(from == null || from.AccessLevel == AccessLevel.Player) return null;

			string msg = null;

			if(Charges > 0)
			{
				msg = String.Format("{0} : {1} secs between uses, {2} charges left",Message,Refractory.TotalSeconds, Charges);
			} 
			else
			{
				msg = String.Format("{0} : {1} secs between uses",Message,Refractory.TotalSeconds);
			}
            
			if(ActivationWord == null)
			{
				return msg;
			} 
			else
			{
				return String.Format("{0} : trigger on '{1}'",msg,ActivationWord);
			}

		}
		
		public override bool HandlesOnSpeech { get { return (ActivationWord != null); } }

		public override void OnSpeech(SpeechEventArgs e )
		{
			base.OnSpeech(e);
		    
			if(e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player) return;

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
				if(AttachedTo is Mobile && Utility.InRange( e.Mobile.Location, ((Mobile)AttachedTo).Location, proximityrange ))
			{
				OnTrigger(null, e.Mobile);
			} 
			else
				return;
		}


		public override void OnTrigger(object activator, Mobile m)
		{
			if(m == null ) return;

			if(DateTime.Now < m_EndTime) return;


			// display a message over the item it was attached to
			if(AttachedTo is Item )
			{
				((Item)AttachedTo).PublicOverheadMessage( MessageType.Regular, 0x3B2, true, Message );
			} 
			else
				if(AttachedTo is Mobile )
			{
				((Mobile)AttachedTo).PublicOverheadMessage( MessageType.Regular, 0x3B2, true, Message );
			}
            
			Charges--;

			// remove the attachment either after the charges run out or if refractory is zero, then it is one use only
			if(Refractory == TimeSpan.Zero || Charges == 0)
			{
				Delete();
			} 
			else
			{
				m_EndTime = DateTime.Now + Refractory;
			}
		}
	}
}
