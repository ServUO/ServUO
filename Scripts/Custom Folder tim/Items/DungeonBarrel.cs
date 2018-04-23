#region Header
// **********
// [AdaptUO] - Dungeon Barrel
// A chop-able barrel that will spawn a variety
// of configurable loot or mobiles.
// **********
#endregion

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Items;
using Server.Mobiles;
#endregion

namespace Server.Items
{
    public class DungeonBarrel : Container, IChopable
    {
        public override int DefaultMaxWeight{ get{ return 0; } } // A value of 0 signals unlimited weight

        public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
        {
            if ( from.AccessLevel < AccessLevel.GameMaster  )
            {
                from.SendLocalizedMessage( 501747 ); // It appears to be locked.
                return false;
            }
            return base.TryDropItem( from, dropped, sendFullMessage );
        }

        public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
        {
            if ( from.AccessLevel < AccessLevel.GameMaster  )
            {
                from.SendLocalizedMessage( 501747 ); // It appears to be locked.
                return false;
            }

            return base.OnDragDropInto( from, item, p );
        }

        public override void OnDoubleClick( Mobile from)
        {
            from.SendLocalizedMessage(501747); // It appears to be locked.
        }

        public override int DefaultGumpID{ get{ return 0x3E; } }
        public override int DefaultDropSound{ get{ return 0x42; } }

        public override Rectangle2D Bounds
        {
            get{ return new Rectangle2D( 33, 36, 109, 112 ); }
        }

        [Constructable]
        public DungeonBarrel() : base( 0xFAE) //0xE77 )  
        {
            Name = "Sealed Barrel";
            Hue = 1121;
            Movable = false;
        }

        public DungeonBarrel(Serial serial) : base(serial)
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

        public void OnChop( Mobile from )
        {
            if ( from.InRange( this.GetWorldLocation(), 2 ) )
            {
                Effects.SendLocationEffect( Location, Map, 0x3728, 20, 10); //smoke or dust
                Effects.PlaySound( Location, Map, 0x11C );

                switch (Utility.Random(12))
                {
                    case 0:
                        Effects.SendLocationEffect( from, from.Map, 0x113A, 20, 10 ); //Posion Player
                        from.PlaySound( 0x231 );
                        from.ApplyPoison( from, Poison.Regular );
                        break;
                    case 1:
                        Effects.SendLocationEffect(from, from.Map, 0x3709, 30); //Burn Player
                        from.PlaySound(0x54);
                        AOS.Damage(from, from, Utility.RandomMinMax(10, 40), 0, 100, 0, 0, 0);
                        break;
                    case 2:
                        new BarrelLid().MoveToWorld(Location, Map);
                        new BarrelHoops().MoveToWorld(Location, Map);
                        break;
                    case 3:
                        Bandage b = new Bandage(Utility.RandomMinMax(50, 100)); 
                        b.MoveToWorld(Location, Map);
                        break;
                    case 4:
                        new BarrelStaves().MoveToWorld(Location, Map);
                        new BarrelHoops().MoveToWorld(Location, Map);
                        break;
                    case 5:
                        Gold g = new Gold(Utility.RandomMinMax(100,1000)); 
                        g.MoveToWorld(Location, Map);
                        break;
                    case 6:
                        new CurePotion().MoveToWorld(Location, Map);
                        break;
                    case 7:
                        new GreaterCurePotion().MoveToWorld(Location, Map);
                        break;
                    case 8:
                        new HealPotion().MoveToWorld(Location, Map);
                        break;
                    case 9:
                        new GreaterHealPotion().MoveToWorld(Location, Map);
                        break;
                    case 10:
                        CoralSnake S1 = new CoralSnake();
                        CoralSnake S2 = new CoralSnake();
                        S1.MoveToWorld(new Point3D(((DungeonBarrel)this).X, ((DungeonBarrel)this).Y, ((DungeonBarrel)this).Z), ((DungeonBarrel)this).Map);
                        S2.MoveToWorld(new Point3D(((DungeonBarrel)this).X, ((DungeonBarrel)this).Y, ((DungeonBarrel)this).Z), ((DungeonBarrel)this).Map);
                        from.SendMessage( "The barrel was infested with snakes!" );
                        break;
                    case 11:
                        RottingCorpse S3 = new RottingCorpse();
                        S3.MoveToWorld(new Point3D(((DungeonBarrel)this).X, ((DungeonBarrel)this).Y, ((DungeonBarrel)this).Z), ((DungeonBarrel)this).Map);
                        from.SendMessage( "You have awakened a rotting corpse!" );
                        break;
                }
                Destroy();                
            }
                else
            {
                from.SendLocalizedMessage( 500446 ); // That is too far away.
            }
        }
    }
}