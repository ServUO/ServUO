using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
   public class ArenaBankStone : Item
   {
      [Constructable]
      public ArenaBankStone() : base( 3796 )
      {
         Movable = false;
         Name = " Arena Bank Stone";
		 Hue = Utility.RandomList( 0, 1153, 1175, 1157 );
         LootType=LootType.Blessed;
      }

      public override void OnDoubleClick( Mobile from )
      {

	PlayerMobile pm = from as PlayerMobile;

			
				if ( from.Criminal )
				{
					from.SendMessage( "Thou art a criminal and cannot access thy bank box." );
				}

				else
				{

				BankBox box = from.BankBox;

				if ( box != null )
				box.Open();
				}
 
		        
		        { 
		            from.SendLocalizedMessage( 500446 ); // That is too far away. 
		        }
      }

      public ArenaBankStone( Serial serial ) : base( serial )
      {
      }

      public override void Serialize( GenericWriter writer )
      {
         base.Serialize( writer );

         writer.Write( (int) 0 ); // version
      }

      public override void Deserialize( GenericReader reader )
      {
         base.Deserialize( reader );

         int version = reader.ReadInt();
      }
   }
}
