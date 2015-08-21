using System;
using Server.Gumps;

/*
** PointsRewardStone
** ArteGordon
** updated 11/08/04
**
** used to open the PointsRewardGump that allows players to purchase rewards with their XmlPoints pvp Credits.
*/

namespace Server.Items
{
    public class PointsRewardStone : Item
    {
        [Constructable]
        public PointsRewardStone() : base( 0xED4 )
        {
            Movable = false;
            Name = "a Points Reward Stone";
        }

        public PointsRewardStone( Serial serial ) : base( serial )
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
                from.SendGump( new PointsRewardGump( from, 0 ) );
            }
            else
            {
                from.SendLocalizedMessage( 500446 ); // That is too far away.
            }
        }
    }
} 
