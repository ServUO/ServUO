using System;
using Server.Gumps;

/*
** MobFactionsRewardStone
** ArteGordon
** updated 11/08/04
**
** used to open the MobFactionsRewardGump that allows players to purchase rewards with their MobFactions kill Credits.
*/

namespace Server.Items
{
    public class MobFactionsRewardStone : Item
    {
        [Constructable]
        public MobFactionsRewardStone() : base( 0xED4 )
        {
            Movable = false;
            Name = "a MobFactions Reward Stone";
        }

        public MobFactionsRewardStone( Serial serial ) : base( serial )
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
            if ( from.InRange( GetWorldLocation(), 2 ) )
            {
                from.SendGump( new MobFactionsRewardGump( from, 0 ) );
            }
            else
            {
                from.SendLocalizedMessage( 500446 ); // That is too far away.
            }
        }
    }
} 
