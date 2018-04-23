using System;
using Server.Items;
using Server.Targeting;
using Server.Gumps;
using Server.ContextMenus;
using Server.Mobiles;
using System.Collections;
using Server.Network;
using Server.Multis;
namespace Server.Items
{
   public class PetRes : Item
   {
   	private Mobile m_Patient;
        private PetRes m_Item;
      [Constructable]
      public PetRes() : base( 0xED4 )
      {
         Movable = true;
         Hue = 0x250;
         Weight = 100;
         Name = "a Pet Res Stone";
      }

      public PetRes( Serial serial ) : base( serial )
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

      public override void OnDoubleClick( Mobile from )
      {
      if ( this.Movable == true )
      {
       from.SendMessage ("This must be locked down to use!");
       return;
      }
      
      		if ( from.InRange( GetWorldLocation(), Core.AOS ? 2 : 1 ) )
			{
				from.RevealingAction();

				from.SendMessage( "What pet would you like to resurrect?" );
                from.Target = new PetresTarget();
			}
			else
			{
	 			from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
			}
      }
      public class PetresTarget : Target
      {
          public PetresTarget()
              : base(2, false, TargetFlags.Beneficial)
          {
          }
          protected override void OnTarget(Mobile m, object targeted)
          {
             /* if (targeted is BaseSoldier)
              {
                  BaseCreature pp = targeted as BaseCreature;
                  pp.PlaySound(0x214);
                  pp.FixedEffect(0x376A, 10, 16);
                  m.SendMessage("Yep getting here");
                  m.SendGump(new PetResurrectGump(m, pp));
                  //pp.Resurrect();
              }
              else*/ if (!m.Player && (m.Body.IsMonster || m.Body.IsAnimal))
              {
                  BaseCreature petPatient = targeted as BaseCreature;

                  m.PlaySound(0x214);
                  m.FixedEffect(0x376A, 10, 16);

                  if (petPatient != null && petPatient.IsDeadPet)
                  {
                      Mobile master = petPatient.ControlMaster;

                      if (master != null && master.InRange(petPatient, 3))
                      {
                          master.SendGump(new PetResurrectGump(m, petPatient));
                      }
                  }
              }

              else
              {
                  m.SendMessage("That cannot be resurrected with this stone");
              }
          }
      }

   }
}
