using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;

namespace Server.Items
{
   public class ControlGem : Item
   {
      private TownCrierStone m_Stone;

      [CommandProperty( AccessLevel.Seer )]
      public TownCrierStone Stone{ get{ return m_Stone; } set{ m_Stone = value; } }

		public ControlGem() : base( 0x186A )
      {
         Name = "TownCrier Controls";
         m_Stone = null;
      }

      public override void OnDoubleClick( Mobile from )
      {
         TownCrierStone stone = m_Stone as TownCrierStone;
         if ( stone != null)
         {
         if ( from.AccessLevel >= AccessLevel.Seer )
            from.SendGump( new TownCrierbGump( stone, null ) );
			}
			else
			{
            from.SendMessage( 0x35, "This gem is not yet linked to the TownCrierb Control Stone." );
			}
      }

      public ControlGem( Serial serial ) : base( serial )
      {
      }

      public override void Serialize( GenericWriter writer )
      {
         base.Serialize( writer );

         writer.Write( (int) 0 ); // version
			writer.Write( m_Stone );
      }

      public override void Deserialize( GenericReader reader )
      {
         base.Deserialize( reader );

         int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_Stone = reader.ReadItem() as TownCrierStone;

					break;
				}
			}
      }
   }
}
