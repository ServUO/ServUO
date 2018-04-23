using System;
using System.IO;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class TownCrierStone : Item
	{
		public ArrayList m_Registry;
		private bool m_Active;
		private bool m_Random;
		private TimeSpan m_Delay;

		[CommandProperty( AccessLevel.Seer )]
		public bool Active
      {
         get{ return m_Active; }
         set
         {
            m_Active = value;
            this.OnActivate();
         }
      }
		
		[CommandProperty( AccessLevel.Seer )]
		public bool Random{ get{ return m_Random; } set { m_Random = value; } }
		
		[CommandProperty( AccessLevel.Seer )]
		public TimeSpan Delay{ get{ return m_Delay; } set { m_Delay = value; } }
		
		public ArrayList Registry{ get{ return m_Registry; } }

		[Constructable]
		public TownCrierStone() : base( 0xEDC )
		{
         m_Registry = new ArrayList();
			m_Active = false;
			m_Random = false;
			m_Delay = new TimeSpan( 0, 0, 10 );

         this.Visible = false;
			this.Movable = false;
			this.Name = "TownCrier Control Stone";

		}

		public override void OnDelete()
		{
 			foreach ( TownCrierb crier in m_Registry )
			{
            TownCrierb m_Crier = crier as TownCrierb;

            if ( Active )
               Active = false;

            m_Crier.Stone = null;
         }

			base.OnDelete();
		}

      public virtual void OnActivate()
      {
 			   foreach ( TownCrierb crier in m_Registry )
			   {
               TownCrierb m_Crier = crier as TownCrierb;
               m_Crier.StoneActive = Active;
            }
      }

		public override void OnDoubleClick( Mobile from )
		{
         if ( from.AccessLevel >= AccessLevel.Seer )
            from.SendGump( new TownCrierbGump( this, null ) );
		}

		public TownCrierStone( Serial serial ) : base( serial )
		{
		}

		public TownCrierStone( GenericReader reader )
		{
			Deserialize( reader );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version

			writer.Write( m_Active );
			writer.Write( m_Random );
			writer.Write( m_Delay );
			writer.WriteMobileList( m_Registry );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Active = reader.ReadBool();
					m_Random = reader.ReadBool();
					m_Delay = reader.ReadTimeSpan();
					m_Registry = reader.ReadMobileList();

					if ( m_Active )
                  		OnActivate();					

					break;
				}
			}
		}
	}
}
