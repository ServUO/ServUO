using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
   public abstract class BaseAtomicBomb : BasePotion
   {
      public abstract int MinDamage { get; }
      public abstract int MaxDamage { get; }

      public override bool RequireFreeHand{ get{ return false; } }

      private static bool LeveledExplosion = false; // Should explosion potions explode other nearby potions?
      private static bool InstantExplosion = false; // Should explosion potions explode on impact?
      private const int   ExplosionRange   = 10;     // How long is the blast radius?

      public BaseAtomicBomb( PotionEffect effect ) : base( 0xF0D, effect )
      {
      }

      public BaseAtomicBomb( Serial serial ) : base( serial )
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

      public override void Drink( Mobile from )
      {
         from.RevealingAction();

         from.SendLocalizedMessage( 500236 ); // You should throw it now!
         from.Target = new InternalTarget( this );

         new DelayTimer( from, this ).Start();
      }

      public void Explode( Mobile from, bool direct, Point3D loc, Map map )
      {
         if ( Deleted )
            return;

         Delete();

         if ( map == null )
            return;

         Effects.PlaySound( loc, map, 0x207 );
         for( int i=0; i<20;i++)
         {
            Point3D temp1 = new Point3D( loc.X, loc.Y, (loc.Z + i));
            Effects.SendLocationEffect( temp1, map, 0x3709, 60 );
         }

         int alchemyBonus = direct ? (int)(from.Skills[SkillName.Alchemy].Value / 10) : 0;

         IPooledEnumerable eable = LeveledExplosion ? map.GetObjectsInRange( loc, ExplosionRange ) : map.GetMobilesInRange( loc, ExplosionRange );
         ArrayList toExplode = new ArrayList();

         foreach ( object o in eable )
         {
            if ( o is Mobile )
            {
               toExplode.Add( o );
            }
            else if ( o is BaseAtomicBomb && o != this )
            {
               toExplode.Add( o );
            }
         }

         eable.Free();

         for ( int i = 0; i < toExplode.Count; ++i )
         {
            object o = toExplode[i];

            if ( o is Mobile )
            {
               Mobile m = (Mobile)o;

               if ( from == null || (SpellHelper.ValidIndirectTarget( from, m ) && from.CanBeHarmful( m, false )) )
               {
                  if ( from != null )
                     from.DoHarmful( m );

                  int damage = Utility.RandomMinMax( MinDamage, MaxDamage );

                  damage += alchemyBonus;

                  if ( damage > 40 )
                     damage = 40;

                  AOS.Damage( from, damage, 0, 100, 0, 0, 0 );
               }
            }
            else if ( o is BaseAtomicBomb )
            {
               BaseAtomicBomb pot = (BaseAtomicBomb)o;

               pot.Explode( from, false, pot.GetWorldLocation(), pot.Map );
            }
         }
         if ( map != null )
         {
            for ( int x = -8; x <= 8; ++x )
            {
               for ( int y = -8; y <= 8; ++y )
               {
                  double dist = Math.Sqrt(x*x+y*y);

                  if ( dist <= 8 )
                  {
                     Explotion( loc, map, X + x, Y + y );
                  }
               }
            }
         }

      }
      public void Explotion( Point3D location, Map m_Map, int x, int y )
      {
         Point3D loc = location;
         Map map = m_Map;
         int m_X = x;
         int m_Y = y;

         int z = map.GetAverageZ( m_X, m_Y );
         bool canFit = map.CanFit( m_X, m_Y, z, 6, false, false );

         for ( int i = -3; !canFit && i <= 3; ++i )
         {
            canFit = map.CanFit( m_X, m_Y, z + i, 6, false, false );

            if ( canFit )
               z += i;
         }

         if ( !canFit )
            return;

         Point3D temp1 = new Point3D( m_X, m_Y, z );
         Effects.SendLocationEffect( temp1, map, 0x3709, 60 );
      }
      private class InternalTarget : Target
      {
         private BaseAtomicBomb m_Potion;

         public InternalTarget( BaseAtomicBomb potion ) : base( 12, true, TargetFlags.None )
         {
            m_Potion = potion;
         }

         protected override void OnTarget( Mobile from, object o )
         {
            if ( m_Potion.Deleted )
               return;

            IPoint3D p = o as IPoint3D;

            if ( p != null )
            {
               if ( p is Item )
                  p = ((Item)p).GetWorldTop();

               from.RevealingAction();

               new InternalTimer( from, m_Potion, p ).Start();

               IEntity to;

               if ( p is Mobile )
                  to = (Mobile)p;
               else
                  to = new Entity( Serial.Zero, new Point3D( p ), from.Map );

               Effects.SendMovingEffect( from, to, m_Potion.ItemID & 0x3FFF, 7, 0, false, false, m_Potion.Hue, 0 );
            }
         }

         private class InternalTimer : Timer
         {
            private Mobile m_Mobile;
            private BaseAtomicBomb m_Potion;
            private IPoint3D m_Point;
            private Map m_Map;

            public InternalTimer( Mobile mobile, BaseAtomicBomb potion, IPoint3D p ) : base( TimeSpan.FromSeconds( 1.0 ) )
            {
               m_Mobile = mobile;
               m_Potion = potion;
               m_Point = p;
               m_Map = mobile.Map;
            }

            protected override void OnTick()
            {
               if ( m_Point is Item && (((Item)m_Point).Deleted || ((Item)m_Point).Map != m_Map) )
                  return;
               else if ( m_Point is Mobile && (((Mobile)m_Point).Deleted || ((Mobile)m_Point).Map != m_Map) )
                  return;
               else if ( m_Map == null )
                  return;

               if ( m_Point is Item )
                  m_Point = ((Item)m_Point).GetWorldLocation();

               if ( InstantExplosion )
                  m_Potion.Explode( m_Mobile, true, new Point3D( m_Point ), m_Map );
               else
                  m_Potion.MoveToWorld( new Point3D( m_Point ), m_Map );
            }
         }
      }

      private class DelayTimer : Timer
      {
         private Mobile m_Mobile;
         private BaseAtomicBomb m_Potion;
         private int m_Index;

         public DelayTimer( Mobile mobile, BaseAtomicBomb potion ) : base( TimeSpan.FromSeconds( 0.75 ), TimeSpan.FromSeconds( 1.0 ) )
         {
            Priority = TimerPriority.FiftyMS;

            m_Mobile = mobile;
            m_Potion = potion;
            m_Index = 3;

            m_Potion.Internalize();
         }

         protected override void OnTick()
         {
            if ( m_Potion.Deleted )
            {
               Stop();
            }
            else if ( m_Index == 0 )
            {
               if ( m_Potion.Map == Map.Internal )
                  m_Potion.Explode( m_Mobile, true, m_Mobile.Location, m_Mobile.Map );
               else
                  m_Potion.Explode( m_Mobile, true, m_Potion.Location, m_Potion.Map );

               Stop();

               if ( m_Mobile.Target is InternalTarget )
                  Target.Cancel( m_Mobile );
            }
            else
            {
               if ( m_Potion.Map == Map.Internal )
                  m_Mobile.PublicOverheadMessage( MessageType.Regular, 0x22, false, m_Index.ToString() );
               else
                  m_Potion.PublicOverheadMessage( MessageType.Regular, 0x22, false, m_Index.ToString() );

               --m_Index;
            }
         }
      }
   }
}