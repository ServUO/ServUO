using System; 
using Server; 
using Server.Gumps; 
using Server.Network; 
using Server.Misc; 
using Server.Mobiles; 
using Server.Targeting; 

namespace Server.Items 
{ 
   public class EtherealMountDyeTub : DyeTub 
   { 
	//public override CustomHuePicker CustomHuePicker{ get{ return CustomHuePicker.LeatherDyeTub; } }

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

      [Constructable] 
      public EtherealMountDyeTub() 
      { 
         Weight = 10.0; 
         Redyable = true; 
         Name = "Ethereal Mount Dye Tub";
	 LootType = LootType.Blessed;

      } 

      public EtherealMountDyeTub( Serial serial ) : base( serial ) 
      { 
      } 

      public override void OnDoubleClick( Mobile from ) 
      { 

	PlayerMobile pm = from as PlayerMobile;

			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if( from.InRange( this.GetWorldLocation(), 1 ) ) 
		        { 
		        from.SendMessage( "Select the Ethereal Mount Statuette to dye." ); 
		        from.Target = new InternalTarget( this ); 
		        } 
		        else 
		        { 
		            from.SendLocalizedMessage( 500446 ); // That is too far away. 
		        } 
	} 


      private class InternalTarget : Target 
      { 
         private EtherealMountDyeTub m_LTub; 

         public InternalTarget( EtherealMountDyeTub tub ) : base( 1, false, TargetFlags.None ) 
         { 
            m_LTub = tub; 
         } 

         protected override void OnTarget( Mobile from, object targeted ) 
         { 
            if ( targeted is EtherealMount ) 
            { 
               EtherealMount EtherealMount = targeted as EtherealMount; 
               if ( !from.InRange( m_LTub.GetWorldLocation(), 1 ) || !from.InRange( ((Item)targeted).GetWorldLocation(), 1 ) ) 
               { 
                  from.SendLocalizedMessage( 500446 ); // That is too far away. 
               } 
               else if (( ((Item)targeted).Parent != null ) && ( ((Item)targeted).Parent is Mobile ) ) 
               { 
                  from.SendMessage( "You cannot dye that in it's current location." ); 
               } 
                  EtherealMount.Hue = m_LTub.Hue; 
                  from.PlaySound( 0x23E ); 
               } 
            } 
         } 
      } 
   } 