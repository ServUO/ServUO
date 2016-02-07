using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlSound : XmlAttachment
	{
		private int m_SoundValue = 500;    // default sound
		private string m_Word = null;     // no word activation by default
		private TimeSpan m_Refractory = TimeSpan.FromSeconds(5);    // 5 seconds default time between activations
		private DateTime m_EndTime;
		private int m_Charges = 0;                        // no charge limit
		private int proximityrange = 5;                 // default movement activation from 5 tiles away

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range { get { return proximityrange; } set { proximityrange  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int SoundValue { get { return m_SoundValue; } set { m_SoundValue  = value; } }

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
		public XmlSound(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlSound()
		{
		}

		[Attachable]
		public XmlSound(int sound)
		{
			SoundValue = sound;
		}

		[Attachable]
		public XmlSound(int sound, double refractory)
		{
			SoundValue = sound;
			Refractory = TimeSpan.FromSeconds(refractory);
		}

		[Attachable]
		public XmlSound(int sound, double refractory, string word )
		{
			ActivationWord = word;
			SoundValue = sound;
			Refractory = TimeSpan.FromSeconds(refractory);
		}
        
		[Attachable]
		public XmlSound(int sound, double refractory, string word, int charges )
		{
			ActivationWord = word;
			SoundValue = sound;
			Refractory = TimeSpan.FromSeconds(refractory);
			Charges = charges;
		}
        
		[Attachable]
		public XmlSound(int sound, double refractory, int charges )
		{
			SoundValue = sound;
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
			writer.Write(m_SoundValue);
			writer.Write(m_Word);
			writer.Write(m_Charges);
			writer.Write(m_Refractory);
			writer.Write(m_EndTime - DateTime.UtcNow);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch(version)
			{
				case 1:
					// version 1
					proximityrange = reader.ReadInt();
					goto case 0;
				case 0:
					// version 0
					SoundValue = reader.ReadInt();
					ActivationWord = reader.ReadString();
					Charges = reader.ReadInt();
					Refractory = reader.ReadTimeSpan();
					TimeSpan remaining = reader.ReadTimeSpan();
					m_EndTime = DateTime.UtcNow + remaining;
					break;
			}
		}

		public override string OnIdentify(Mobile from)
		{
			if(from == null || from.AccessLevel == AccessLevel.Player) return null;

			string msg = null;

			if(Charges > 0)
			{
				msg = String.Format("Sound #{0} : {1} secs between uses - {2} charges left",SoundValue,Refractory.TotalSeconds, Charges);
			} 
			else
			{
				msg = String.Format("Sound #{0} : {1} secs between uses",SoundValue,Refractory.TotalSeconds);
			}

			if(ActivationWord == null)
			{
				return msg;
			} 
			else
			{
				return String.Format("{0} : trigger on '{1}'",msg, ActivationWord);
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
				return;
		}


		public override void OnTrigger(object activator, Mobile m)
		{
			if(m == null ) return;

			if(DateTime.UtcNow < m_EndTime) return;


			// play a sound
			if(AttachedTo is Mobile )
			{
				try
				{
					Effects.PlaySound(((Mobile)AttachedTo).Location, ((IEntity)AttachedTo).Map,  SoundValue);
				} 
				catch{}
			} 
			else
				if(AttachedTo is Item )
			{
				Item i = AttachedTo as Item;

				if(i.Parent == null)
				{
					try
					{
						Effects.PlaySound(i.Location, i.Map,  SoundValue);
					} 
					catch{}
				} 
				else
					if(i.RootParent is IEntity)
				{
					try
					{
						Effects.PlaySound(((IEntity)i.RootParent).Location, ((IEntity)i.RootParent).Map,  SoundValue);
					} 
					catch{}
				}
			}

			Charges--;

			// remove the attachment either after the charges run out or if refractory is zero, then it is one use only
			if(Refractory == TimeSpan.Zero || Charges == 0)
			{
				Delete();
			} 
			else
			{
				m_EndTime = DateTime.UtcNow + Refractory;
			}
		}
	}
}
