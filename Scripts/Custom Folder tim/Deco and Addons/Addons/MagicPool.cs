using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Misc;
using System.Collections;
using Server.ContextMenus;

namespace Server.Items
{
	public class MagicPool : BaseAddon
	{
		private int m_Pool;
		private int m_Uses;
		private int m_Bonus;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Pool
		{
			get{ return m_Pool; }
			set{ m_Pool = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Uses
		{
			get{ return m_Uses; }
			set{ m_Uses = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Bonus
		{
			get{ return m_Bonus; }
			set{ m_Bonus = value; InvalidateProperties(); }
		}

		private DateTime m_DecayTime;
		private Timer m_DecayTimer;

		public virtual TimeSpan DecayDelay{ get{ return TimeSpan.FromMinutes( 30.0 ); } } // HOW LONG UNTIL THE POOL DECAYS IN MINUTES

		[ Constructable ]
		public MagicPool()
		{
			int pool_type = Utility.Random( 8 );

			if ( pool_type == 1 )
			{ 
				AddComplexComponent( (BaseAddon) this, 767, -1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 768, -1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 8669, 0, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 8669, 1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 8669, 0, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 767, -1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 767, 1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 765, 1, 1, 2, 0, -1, "magic pool", 1);
			} 
			else if ( pool_type == 2 )
			{ 
				AddComplexComponent( (BaseAddon) this, 273, -1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 271, 0, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 271, 0, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 272, -1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 272, -1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 271, 1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 272, 1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 270, 1, 1, 2, 0, -1, "magic pool", 1);
			} 
			else if ( pool_type == 3 ) 
			{ 
				AddComplexComponent( (BaseAddon) this, 48, -1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 47, 0, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 47, 0, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 46, -1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 46, -1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 47, 1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 46, 1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 45, 1, 1, 2, 0, -1, "magic pool", 1);
			} 
			else if ( pool_type == 4 ) 
			{ 
				AddComplexComponent( (BaseAddon) this, 700, -1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 699, -1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 699, -1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 698, 0, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 698, 0, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 698, 1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 699, 1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 697, 1, 1, 2, 0, -1, "magic pool", 1);
			} 
			else if ( pool_type == 5 ) 
			{ 
				AddComplexComponent( (BaseAddon) this, 223, -1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 222, 0, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 222, 0, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 221, -1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 221, -1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 222, 1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 221, 1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 220, 1, 1, 2, 0, -1, "magic pool", 1);
			} 
			else if ( pool_type == 6 ) 
			{ 
				AddComplexComponent( (BaseAddon) this, 108, -1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 105, 0, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 105, 0, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 106, -1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 106, -1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 107, 1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 105, 1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 106, 1, 0, 2, 0, -1, "magic pool", 1);
			} 
			else if ( pool_type == 7 ) 
			{ 
				AddComplexComponent( (BaseAddon) this, 490, -1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 490, -1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 491, -1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 488, 1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 489, 0, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 489, 0, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 489, 1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 490, 1, 0, 2, 0, -1, "magic pool", 1);
			} 
			else
			{
				AddComplexComponent( (BaseAddon) this, 10585, -1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 10579, 0, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 10579, 1, -1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 10576, -1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 10576, -1, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 10576, 1, 0, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 10579, 0, 1, 2, 0, -1, "magic pool", 1);
				AddComplexComponent( (BaseAddon) this, 10582, 1, 1, 2, 0, -1, "magic pool", 1);
			}
			AddComplexComponent( (BaseAddon) this, 14186, 1, 1, 7, 0, -1, "magic pool", 1);
			AddComplexComponent( (BaseAddon) this, 6039, 0, 0, 2, 0, -1, "magic pool", 1);
			AddComplexComponent( (BaseAddon) this, 6039, 1, 0, 2, 0, -1, "magic pool", 1);
			AddComplexComponent( (BaseAddon) this, 6039, 0, 1, 2, 0, -1, "magic pool", 1);
			AddComplexComponent( (BaseAddon) this, 6039, 1, 1, 2, 0, -1, "magic pool", 1);

			m_Pool = Utility.Random( 10 );
				if ( Utility.Random( 20 ) == 1 ) { m_Pool = 100; } // TREASURE CHEST
			m_Uses = Utility.RandomMinMax( 1, 10 );
			m_Bonus = Utility.RandomMinMax( 3, 10 );

			RefreshDecay( true );
			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( CheckAddComponents ) );
		}

		public MagicPool( Serial serial ) : base( serial )
		{
		}

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (name != null && name.Length > 0)
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType) lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
        }

		public virtual bool Apply( Mobile from, StatType Type, int Bonus )
		{
			bool applied = Spells.SpellHelper.AddStatOffset( from, Type, Bonus, TimeSpan.FromMinutes( 2.0 ) );

			if ( !applied )
				from.SendLocalizedMessage( 502173 ); // You are already under a similar effect.

			return applied;
		}

		public override void OnComponentUsed( AddonComponent ac, Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 3 ) )
			{
				from.SendMessage( "You will have to get closer to drink from the magical pool!" );
			}
			else if ( m_Uses > 0 )
			{
				if ( m_Pool == 1 ) // STRENGTH
				{
					if ( Apply( from, StatType.Str, m_Bonus ) )
					{
						from.FixedEffect( 0x375A, 10, 15 );
						from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
						from.SendMessage( "You feel stronger after drinking from the pool!" );
						this.m_Uses = this.m_Uses - 1;
					}
				}
				else if ( m_Pool == 2 ) // INTELLECT
				{
					if ( Apply( from, StatType.Int, m_Bonus ) )
					{
						from.FixedEffect( 0x375A, 10, 15 );
						from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
						from.SendMessage( "You can think much more clearly after drinking from the pool!" );
						this.m_Uses = this.m_Uses - 1;
					}
				}
				else if ( m_Pool == 3 ) // DEXTERITY
				{
					if ( Apply( from, StatType.Dex, m_Bonus ) )
					{
						from.FixedEffect( 0x375A, 10, 15 );
						from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
						from.SendMessage( "You feel more nimble after drinking from the pool!" );
						this.m_Uses = this.m_Uses - 1;
					}
				}
				else if ( m_Pool == 4 ) // CURE
				{
					from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
					if ( from.Poisoned )
					{
						from.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
						from.CurePoison( from );
						from.SendMessage( "You feel much better after drinking from the pool!" );
						this.m_Uses = this.m_Uses - 1;
					}
					else
					{
						from.SendMessage( "You drink from the pool and nothing happens!" );
						this.m_Uses = this.m_Uses - 1;
					}
				}
				else if ( m_Pool == 5 ) // HEAL
				{
					from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
					if ( from.Hits < from.HitsMax )
					{
						if ( from.Poisoned || MortalStrike.IsWounded( from ) )
						{
							from.SendMessage( "You drink from the pool and nothing happens!" );
							this.m_Uses = this.m_Uses - 1;
						}
						else
						{
							from.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
								int min = 6;
								int max = 10;
								if ( m_Bonus > 8 ) { min = 20; max = 30; }
								else if ( m_Bonus > 5 ) { min = 13; max = 20; }
							from.Heal( Utility.RandomMinMax( min, max ) );
							from.SendMessage( "You drink from the pool and your wounds magically heal!" );
							this.m_Uses = this.m_Uses - 1;
						}
					}
					else
					{
						from.SendMessage( "You drink from the pool and nothing happens!" );
						this.m_Uses = this.m_Uses - 1;
					}
				}
				else if ( m_Pool == 6 ) // WATER ELEMENTAL
				{
					from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
					try
					{
						Map map = this.Map;
						BaseCreature bc = (BaseCreature)Activator.CreateInstance( typeof( WaterElemental ) );

						Point3D spawnLoc = this.Location;

						for( int i = 0; i < 10; i++ )
						{
							int x = Location.X + Utility.Random( 4 );
							int y = Location.Y + Utility.Random( 4 );
							int z = Map.GetAverageZ( x, y );

							if( Map.CanSpawnMobile( new Point2D( x, y ), this.Z ) )
								spawnLoc = new Point3D( x, y, this.Z );
							else if( Map.CanSpawnMobile( new Point2D( x, y ), z ) )
								spawnLoc = new Point3D( x, y, z );
						}

						Timer.DelayCall( TimeSpan.FromSeconds( 1 ), delegate()
						{
							bc.Home = Location;
							bc.RangeHome = 5;
							bc.FightMode = FightMode.Closest;
							bc.MoveToWorld( spawnLoc, map );
							bc.ForceReacquire();
						} );
					}
					catch
					{
					}
					from.SendMessage( "A water elemental emerges from the pool!" );
					this.m_Uses = this.m_Uses - 1;
				}
				else if ( m_Pool == 7 ) // GOLD TO LEAD
				{
					from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
					Container cont = from.Backpack;
					Gold m_Gold = (Gold)from.Backpack.FindItemByType( typeof( Gold ) );
					int m_Amount = from.Backpack.GetAmount( typeof( Gold ) );

					if ( cont.ConsumeTotal( typeof( Gold ), m_Amount ) )
					{
						from.AddToBackpack( new LeadCoin( m_Amount ) );
						from.SendMessage( "After drinking from the pool, you notice all of your gold has turned to lead!" );
						Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
					}
					else
					{
						from.SendMessage( "You drink from the pool and nothing happens!" );
					}
					this.m_Uses = this.m_Uses - 1;
				}
				else if ( m_Pool == 8 ) // EQUIPPED ITEM DISAPPEARS
				{
					from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
					this.m_Uses = this.m_Uses - 1;
					int mReturn = 0;
					if ( from.FindItemOnLayer( Layer.OuterTorso ) != null ) { from.FindItemOnLayer( Layer.OuterTorso ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.OneHanded ) != null ) { from.FindItemOnLayer( Layer.OneHanded ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.TwoHanded ) != null ) { from.FindItemOnLayer( Layer.TwoHanded ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.Bracelet ) != null ) { from.FindItemOnLayer( Layer.Bracelet ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.Ring ) != null ) { from.FindItemOnLayer( Layer.Ring ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.Helm ) != null ) { from.FindItemOnLayer( Layer.Helm ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.Arms ) != null ) { from.FindItemOnLayer( Layer.Arms ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.OuterLegs ) != null ) { from.FindItemOnLayer( Layer.OuterLegs ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.Neck ) != null ) { from.FindItemOnLayer( Layer.Neck ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.Gloves ) != null ) { from.FindItemOnLayer( Layer.Gloves ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.Talisman ) != null ) { from.FindItemOnLayer( Layer.Talisman ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.Shoes ) != null ) { from.FindItemOnLayer( Layer.Shoes ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.Cloak ) != null ) { from.FindItemOnLayer( Layer.OneHanded ).Delete(); }
					else if ( from.FindItemOnLayer( Layer.FirstValid ) != null ) { from.FindItemOnLayer( Layer.FirstValid ).Delete(); }
					else
					{
						mReturn = 1;
						from.SendMessage( "You drink from the pool and nothing happens!" );
					}
					if ( mReturn != 1 )
					{
						from.SendMessage( "After drinking from the pool, you notice one of your equipped items disappears!" );
						Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
					}
				}
				else if ( m_Pool == 9 ) // LOSE A STAT POINT
				{
					from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
					this.m_Uses = this.m_Uses - 1;
					int mCurse = 1;

					if ( m_Bonus > 8 )
					{
						if ( from.RawStr > 10 ) { from.RawStr = from.RawStr - 1; from.SendMessage( "You lose a strength after drinking from the pool!" ); }
						else { from.SendMessage( "You drink from the pool and nothing happens!" ); mCurse = 0; }
					}
					else if ( m_Bonus > 5 )
					{
						if ( from.RawDex > 10 ) { from.RawDex = from.RawDex - 1; from.SendMessage( "You lose a dexterity after drinking from the pool!" ); }
						else { from.SendMessage( "You drink from the pool and nothing happens!" ); mCurse = 0; }
					}
					else
					{
						if ( from.RawInt > 10 ) { from.RawInt = from.RawInt - 1; from.SendMessage( "You lose an intelligence after drinking from the pool!" ); }
						else { from.SendMessage( "You drink from the pool and nothing happens!" ); mCurse = 0; }
					}

					if ( mCurse == 1 )
					{
						from.FixedParticles( 0x3779, 1, 15, 9905, 32, 2, EffectLayer.Head );
						from.FixedParticles( 0x37B9, 1, 14, 9502, 32, 5, (EffectLayer)255 );
					}
				}
				else if ( m_Pool == 100 ) // TREASURE CHEST
				{
					int tType = 1;
					int tChest = Utility.Random( 100 );
						if ( tChest > 95 ) { tType = 6; }
						else if ( tChest > 85 ) { tType = 5; }
						else if ( tChest > 70 ) { tType = 4; }
						else if ( tChest > 50 ) { tType = 3; }
						else if ( tChest > 25 ) { tType = 2; }

					from.PlaySound( 0x364 );
					from.AddToBackpack( new PoolChest( tType ) );
					from.SendMessage( "You pull a mystical chest out from the pool!" );
					this.m_Uses = 0;
				}
				else // POISON
				{
					if ( from.Poisoned )
					{
						from.SendMessage( "You are too sick to drink from this pool!" );
					}
					else 
					{
						Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x36B0, 1, 14, 63, 7, 9915, 0 );
						from.PlaySound( Utility.RandomList( 0x30, 0x2D6 ) );
							if ( m_Bonus > 9 ) { from.ApplyPoison( from, Poison.Deadly ); }
							else if ( m_Bonus > 7 ) { from.ApplyPoison( from, Poison.Greater ); }
							else if ( m_Bonus > 4 ) { from.ApplyPoison( from, Poison.Regular ); }
							else { from.ApplyPoison( from, Poison.Lesser ); }
						from.SendMessage( "You feel more sick after drinking from the pool!" );
						this.m_Uses = this.m_Uses - 1;
					}
				}
			}
			else
			{
				from.SendMessage( "The magic from the pool seems to be drained!" );
			}
		}

		public void CheckAddComponents()
		{
			if( Deleted )
				return;
			AddComponents();
		}

		public virtual void AddComponents()
		{
		}

		public virtual void RefreshDecay( bool setDecayTime )
		{
			if( Deleted )
				return;
			if( m_DecayTimer != null )
				m_DecayTimer.Stop();
			if( setDecayTime )
				m_DecayTime = DateTime.Now + DecayDelay;
			m_DecayTimer = Timer.DelayCall( DecayDelay, new TimerCallback( Delete ) );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
			writer.WriteDeltaTime( m_DecayTime );
			writer.WriteEncodedInt( (int) m_Pool );
			writer.WriteEncodedInt( (int) m_Uses );
			writer.WriteEncodedInt( (int) m_Bonus );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
				{
					m_DecayTime = reader.ReadDeltaTime();
					RefreshDecay( false );
					break;
				}
			}
			m_Pool = reader.ReadEncodedInt();
			m_Uses = reader.ReadEncodedInt();
			m_Bonus = reader.ReadEncodedInt();
		}
	}
}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Server.Items
{
	public class LeadCoin : Item
	{
		public override double DefaultWeight
		{
			get { return 0.02; }
		}

		[Constructable]
		public LeadCoin() : this( 1 )
		{
		}

		[Constructable]
		public LeadCoin( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public LeadCoin( int amount ) : base( 0xEF0 )
		{
			Stackable = true;
			Name = "lead";
			Amount = amount;
			Hue = 0x967;
		}

		public LeadCoin( Serial serial ) : base( serial )
		{
		}

		public override int GetDropSound()
		{
			if ( Amount <= 1 )
				return 0x2E4;
			else if ( Amount <= 5 )
				return 0x2E5;
			else
				return 0x2E6;
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
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	[Flipable]
	public class PoolChest : LockableContainer
	{
		private static int[] m_ItemIDs = new int[]
		{
			0x9AB, 0xE40, 0xE41, 0xE7C
		};

		private static int[] m_Hues = new int[]
		{
			0x961, 0x962, 0x963, 0x964, 0x965, 0x966, 0x967, 0x968, 0x969, 0x96A, 0x96B, 0x96C, 
			0x96D, 0x96E, 0x96F, 0x970, 0x971, 0x972, 0x973, 0x974, 0x975, 0x976, 0x977, 0x978, 
			0x979, 0x97A, 0x97B, 0x97C, 0x97D, 0x97E, 0x4AA
		};

		private string m_Name;

		[Constructable]
		public PoolChest( int level ) : base( Utility.RandomList( m_ItemIDs ) )
		{
			Name = "mystical chest";
			Hue = Utility.RandomList( m_Hues );
			Fill( level );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "From A Magical Pool" );
		}

		private static void GetRandomAOSStats( out int attributeCount, out int min, out int max )
		{
			int rnd = Utility.Random( 15 );

			if ( rnd < 1 )
			{
				attributeCount = Utility.RandomMinMax( 2, 6 );
				min = 20; max = 70;
			}
			else if ( rnd < 3 )
			{
				attributeCount = Utility.RandomMinMax( 2, 4 );
				min = 20; max = 50;
			}
			else if ( rnd < 6 )
			{
				attributeCount = Utility.RandomMinMax( 2, 3 );
				min = 20; max = 40;
			}
			else if ( rnd < 10 )
			{
				attributeCount = Utility.RandomMinMax( 1, 2 );
				min = 10; max = 30;
			}
			else
			{
				attributeCount = 1;
				min = 10; max = 20;
			}
		}
		
		public void Flip()
		{
			switch ( ItemID )
			{
				case 0x9AB : ItemID = 0xE7C; break;
				case 0xE7C : ItemID = 0x9AB; break;
				
				case 0xE40 : ItemID = 0xE41; break;
				case 0xE41 : ItemID = 0xE40; break;
			}
		}

		private void Fill( int level )
		{
			TrapType = TrapType.ExplosionTrap;
			TrapPower = level * 25;
			TrapLevel = level;
			Locked = true;

			switch ( level )
			{
				case 1: RequiredSkill = 36; break;
				case 2: RequiredSkill = 76; break;
				case 3: RequiredSkill = 84; break;
				case 4: RequiredSkill = 92; break;
				case 5: RequiredSkill = 100; break;
			}

			LockLevel = RequiredSkill - 10;
			MaxLockLevel = RequiredSkill + 40;

			DropItem( new Gold( level * 200 ) );

			for ( int i = 0; i < level * 2; ++i )
			{
				Item item;

				if ( Core.AOS )
					item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();
				else
					item = Loot.RandomArmorOrShieldOrWeapon();

				if ( item is BaseWeapon )
				{
					BaseWeapon weapon = (BaseWeapon)item;

					if ( Core.AOS )
					{
						int attributeCount;
						int min, max;

						GetRandomAOSStats( out attributeCount, out min, out max );

						BaseRunicTool.ApplyAttributesTo( weapon, attributeCount, min, max );
					}
					else
					{
						weapon.DamageLevel = (WeaponDamageLevel)Utility.Random( 6 );
						weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random( 6 );
						weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random( 6 );
					}

					DropItem( item );
				}
				else if ( item is BaseArmor )
				{
					BaseArmor armor = (BaseArmor)item;

					if ( Core.AOS )
					{
						int attributeCount;
						int min, max;

						GetRandomAOSStats( out attributeCount, out min, out max );

						BaseRunicTool.ApplyAttributesTo( armor, attributeCount, min, max );
					}
					else
					{
						armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random( 6 );
						armor.Durability = (ArmorDurabilityLevel)Utility.Random( 6 );
					}

					DropItem( item );
				}
				else if( item is BaseHat )
				{
					BaseHat hat = (BaseHat)item;

					if( Core.AOS )
					{
						int attributeCount;
						int min, max;

						GetRandomAOSStats( out attributeCount, out min, out  max );

						BaseRunicTool.ApplyAttributesTo( hat, attributeCount, min, max );
					}

					DropItem( item );
				}
				else if( item is BaseJewel )
				{
					int attributeCount;
					int min, max;

					GetRandomAOSStats( out attributeCount, out min, out max );

					BaseRunicTool.ApplyAttributesTo( (BaseJewel)item, attributeCount, min, max );

					DropItem( item );
				}
			}

			for ( int i = 0; i < level; i++ )
			{
				Item item = Loot.RandomPossibleReagent();
				item.Amount = Utility.RandomMinMax( 40, 60 );
				DropItem( item );
			}

			for ( int i = 0; i < level; i++ )
			{
				Item item = Loot.RandomGem();
				DropItem( item );
			}

			for ( int i = 0; i < level; i++ )
			{
				Item item = Loot.RandomWand();
				DropItem( item );
			}

			for ( int i = 0; i < level; ++i )
			{
				if ( Utility.Random( 4 ) == 1 ) { DropItem( Loot.RandomScroll( 0, 15, SpellbookType.Necromancer ) ); }
				else { DropItem( Loot.RandomScroll( 0, 63, SpellbookType.Regular ) ); }
			}

			if ( Utility.Random( 2 ) == 1 ) { DropItem( new TreasureMap( level + 1, ( Utility.RandomBool() ? Map.Felucca : Map.Trammel ) ) ); }
		}

		public PoolChest( Serial serial ) : base( serial )
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