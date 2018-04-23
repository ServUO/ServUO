using System;
using Server;
using Server.Targeting;
using Server.Items;

namespace Server.Items
{

     public class UnivTubTarget : Target
     {
            private Item m_Item;

            public UnivTubTarget( Item item ) : base( 12, false, TargetFlags.None )
            {
                   m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                        if ( targeted is Item )
                        {
                             Item targ = (Item)targeted;
                             if ( !targ.IsChildOf (from.Backpack))
                             {
                                  from.SendMessage( "The item is not in your pack!" );
                             }
                             else
                             {
                                 targ.Hue=m_Item.Hue;
                                 from.PlaySound( 0x23F );
                             }
                        }

            }
     }


     public class UniversalDyeTub : Item
     {

            private bool m_Redyable;


            [Constructable]
            public UniversalDyeTub() : base( 0xFAB )
            {
                   Weight = 0.0;
                   Hue = 0;
                   Name = "Universal Dye Tub";
                   m_Redyable = false;
            }

            public UniversalDyeTub( Serial serial ) : base( serial )
            {
            }

            public override void OnDoubleClick( Mobile from )
            {

                   from.Target = new UnivTubTarget( this );
                   from.SendMessage( "What do you wish to dye?" );

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